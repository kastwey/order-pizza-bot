using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

using OrderPizzaBot.Contracts.Repositories;
using OrderPizzaBot.Entities;
using OrderPizzaBot.Helpers;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Dialogs
{
	public class PizzaSelectionDialog : DialogBase
	{

		private readonly UserState _userState;

		private readonly IPizzaRepository _pizzaRepository;

		private readonly IIngredientRepository _ingredientRepository;

		private readonly IStatePropertyAccessor<OrderInfo> _orderInfo;

		public PizzaSelectionDialog(UserState userState, OrderPizzaRecognizer recognizer, IPizzaRepository pizzaRepository, IIngredientRepository ingredientRepository)
			: base(nameof(PizzaSelectionDialog), userState, recognizer)
		{
			_userState = userState;
			_pizzaRepository = pizzaRepository;
			_ingredientRepository = ingredientRepository;
			_orderInfo = _userState.CreateProperty<OrderInfo>("OrderInfo");
			AddDialog(new TextPrompt("ChoosePizza", ValidatePizza));
			AddDialog(new ChoicePrompt("ChoicePizzaSize", ValidateMaxAttemptsReached, "es"));
			AddDialog(new ChoicePrompt("ChoicePizzaDough", ValidateMaxAttemptsReached, "es"));
			AddDialog(new ConfirmPrompt("ConfirmPizzaConfiguration", ValidateConfirmation, "es"));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				AskForPizzaAsync,
				SetPizzaAsync,
				AskForPizzaSizeAsync,
				AskForDoughTypeAsync,
				ConfirmPizzaAsync,
				EndPizzaAsync,
			}));
			AddDialog(new CustomPizzaDialog(_userState, Recognizer, _pizzaRepository, _ingredientRepository));
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> AskForPizzaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var orderInfo =await  _orderInfo.GetAsync(stepContext.Context);
			var pizzaNumberMsg = $"¡Vamos a por la pizza {orderInfo.ConfiguringPizzaIndex}!";
			await stepContext.Context.SendActivityAsync(pizzaNumberMsg, pizzaNumberMsg, InputHints.IgnoringInput);
			var message = "¿Qué pizza te apetece? Si no recuerdas qué ofrecemos en la carta, puedes pedírnosla. Si quieres crear una pizza personalizada, di: \"personalizada\".";
			var retryMessage = "¡Esa pizza no existe! Si no recuerdas qué pizzas tenemos, pídenos la carta. Si quieres una pizza a tu gusto, di: \"personalizada\".";
			return await stepContext.PromptAsync("ChoosePizza", new PromptOptions
			{
				Prompt = MessageFactory.Text(message, message),
				RetryPrompt = MessageFactory.Text(retryMessage, retryMessage)
			}, cancellationToken);
		}

		private async Task<bool> ValidatePizza(PromptValidatorContext<string> validatorContext, CancellationToken cancellationToken)
		{
			if (await MaxAttemptsReachedAsync(validatorContext, cancellationToken))
			{
				return true;
			}
			
			var value = validatorContext.Recognized.Value;
			if (value.Equals("personalizada", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return true;
			}
			var pizzaExists = _pizzaRepository.GetPizzas()
				.Any(p => value.Contains(p.Name, System.StringComparison.InvariantCultureIgnoreCase));
			if (pizzaExists)
			{
				return true;
			}

			var result = await Recognizer.RecognizeAsync<OrderPizza>(validatorContext.Context, cancellationToken);
			var intent = result.TopIntent();
			return intent.intent == OrderPizza.Intent.GetMenu;
		}

		private async Task<DialogTurnResult> SetPizzaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (await ShouldCancelDialogsAsync(stepContext.Context, cancellationToken))
			{
				return await stepContext.CancelAllDialogsAsync();
			}
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context);
			string name = (string)stepContext.Result;
			if (name.Equals("personalizada", System.StringComparison.InvariantCultureIgnoreCase))
			{
				return await stepContext.BeginDialogAsync(nameof(CustomPizzaDialog), orderInfo);
			}

			var pizza = _pizzaRepository.GetPizzas()
				.FirstOrDefault(p => name.ToLower().Contains(p.Name));
			// if the pizza does not exists, it means that we are printing the menu.
			if (pizza == null)
			{
				await MenuHelper.PrintMenuAsync(stepContext, _pizzaRepository, cancellationToken);
				return await stepContext.ReplaceDialogAsync(nameof(WaterfallDialog), orderInfo, cancellationToken);
			}
			orderInfo.Pizzas.Add(pizza);
			await _orderInfo.SetAsync(stepContext.Context, orderInfo);
			return await stepContext.NextAsync();
		}

		private async Task<DialogTurnResult> AskForPizzaSizeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (await ShouldCancelDialogsAsync(stepContext.Context, cancellationToken))
			{
				return await stepContext.CancelAllDialogsAsync();
			}
			var sizeMessage = "¿De qué tamaño quieres la pizza?";
			var retryMessage = "Solo tenemos pizzas pequeñas, medianas y grandes. Elige uno de estos valores. ¿De qué tamaño quieres la pizza?";

			return await stepContext.PromptAsync("ChoicePizzaSize", new PromptOptions
			{
				Prompt = MessageFactory.Text(sizeMessage, sizeMessage, InputHints.ExpectingInput),
				RetryPrompt = MessageFactory.Text(retryMessage, retryMessage),
				Choices = ChoiceFactory.ToChoices(new List<string> {
					"pequeña", "mediana", "grande"
				})
			}, cancellationToken);
		}

		private async Task<DialogTurnResult> AskForDoughTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (await ShouldCancelDialogsAsync(stepContext.Context, cancellationToken))
			{
				return await stepContext.CancelAllDialogsAsync();
			}
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context);
			var pizza = orderInfo.Pizzas.Last();
			pizza.Size = (PizzaSize)((FoundChoice)stepContext.Result).Index + 1;
			var doughMessage = "¿Qué tipo de masa deseas? ¿fina o normal?";
			var retryMessage = "Solo tenemos masa fina o normal. ¿Qué masa de pizza quieres?";

			return await stepContext.PromptAsync("ChoicePizzaDough", new PromptOptions
			{
				Prompt = MessageFactory.Text(doughMessage, doughMessage, InputHints.ExpectingInput),
				RetryPrompt = MessageFactory.Text(retryMessage, retryMessage),
				Choices = ChoiceFactory.ToChoices(new List<string>
				{
					"fina", "normal"
				})
			}, cancellationToken);
		}

		private async Task<DialogTurnResult> ConfirmPizzaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (await ShouldCancelDialogsAsync(stepContext.Context, cancellationToken))
			{
				return await stepContext.CancelAllDialogsAsync();
			}
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context);
			var pizza = orderInfo.Pizzas.Last();
			pizza.Dough = (DoughType)((FoundChoice)stepContext.Result).Index + 1;
			var confirmationMessage = "Esta es tu pizza: " + pizza.ToString() + ". ¿Es correcto?";
			var retryMessage = $"Por favor, di sí o no. {confirmationMessage}";
			return await stepContext.PromptAsync("ConfirmPizzaConfiguration", new PromptOptions
			{
				Prompt = MessageFactory.Text(confirmationMessage, confirmationMessage, InputHints.ExpectingInput),
				RetryPrompt = MessageFactory.Text(retryMessage, retryMessage)
			}, cancellationToken);
		}

		private async Task<DialogTurnResult> EndPizzaAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			if (await ShouldCancelDialogsAsync(stepContext.Context, cancellationToken))
			{
				return await stepContext.CancelAllDialogsAsync();
			}
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context, null, cancellationToken);
			bool right = (bool)stepContext.Result;
			if (!right)
			{
				string sorryMessage = "¡Oh, vaya, lo siento, te habré entendido mal! ¡Empecemos de nuevo!";
				await stepContext.Context.SendActivityAsync(sorryMessage, sorryMessage, InputHints.IgnoringInput, cancellationToken);
				orderInfo.Pizzas.Remove(orderInfo.Pizzas.Last());
				return await stepContext.ReplaceDialogAsync(nameof(WaterfallDialog), orderInfo, cancellationToken);
			}
			var successMessage = "¡Pizza añadida satisfactoriamente!";
			await stepContext.Context.SendActivityAsync(successMessage, successMessage, InputHints.IgnoringInput, cancellationToken);

			if (orderInfo.NumberOfPizzas > orderInfo.Pizzas.Count)
			{
				orderInfo.ConfiguringPizzaIndex++;
				var nextPizzaMessage = "¡Vamos a por la siguiente pizza!";
				await stepContext.Context.SendActivityAsync(nextPizzaMessage, nextPizzaMessage, InputHints.IgnoringInput, cancellationToken);
				return await stepContext.ReplaceDialogAsync(nameof(WaterfallDialog), orderInfo, cancellationToken);
			}
			return await stepContext.NextAsync();
		}
	}
}