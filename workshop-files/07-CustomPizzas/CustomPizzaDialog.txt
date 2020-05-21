using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using OrderPizzaBot.Contracts.Repositories;
using OrderPizzaBot.Entities;
using OrderPizzaBot.Extensions;

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Dialogs
{
	public class CustomPizzaDialog : DialogBase
	{

		private readonly UserState _userState;

		private readonly IIngredientRepository _ingredientRepository;

		private readonly IStatePropertyAccessor<OrderInfo> _orderInfo;

		public CustomPizzaDialog(UserState userState, OrderPizzaRecognizer recognizer, IIngredientRepository ingredientRepository)
			: base(nameof(CustomPizzaDialog), recognizer)
		{
			_userState = userState;
			_ingredientRepository = ingredientRepository;
			_orderInfo = _userState.CreateProperty<OrderInfo>("OrderInfo");

			AddDialog(new TextPrompt("GetIngredients"));
			AddDialog(new ChoicePrompt("ChoicePizzaSize", null, "es"));
			AddDialog(new ConfirmPrompt("ConfirmIngredients", ValidateConfirmation, "es"));
			AddDialog(new ChoicePrompt("ChoicePizzaDough", null, "es"));
			AddDialog(new ConfirmPrompt("ConfirmPizzaConfiguration", ValidateConfirmation, "es"));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				AskForIngredientsAsync,
				AskForIngredientConfirmationAsync,
				AcceptIngredientsAsync,
			}));
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> AskForIngredientsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var message = "¡Vamos a crear una nueva pizza para ti! Dime los ingredientes que quieras. Si quieres doble de algún ingrediente, dilo dos veces.";
			return await stepContext.PromptAsync("GetIngredients", new PromptOptions
			{
				Prompt = MessageFactory.Text(message, message),
			}, cancellationToken);
		}

		private async Task<DialogTurnResult> AskForIngredientConfirmationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{

			var orderInfo = await _orderInfo.GetAsync(stepContext.Context);
			var userIngredients = (string)stepContext.Result;
			var existingIngredients = _ingredientRepository.GetIngredients().OrderByDescending(i => i.Name.Length).ToList();
			var addedIngredients = new List<Ingredient>();
			foreach (var ingredient in existingIngredients)
			{
				var ingredientIndex = userIngredients.IndexOf(ingredient.Name);
				if (ingredientIndex >= 0)
				{
					addedIngredients.Add(ingredient);
					userIngredients = userIngredients.Remove(ingredientIndex, ingredient.Name.Length);
				}
			}

			if (!addedIngredients.Any())
			{
				string noIngredientsMsg = "Lo siento, pero ninguno de los ingredientes que me has dicho están en nuestra base de datos. Recuerda que los ingredientes que te ofrecemos son: " +
					existingIngredients.OrderBy(i => i.Name).Select(i => i.Name).ToArray().ConcatenateWith("y") + ".";
				await stepContext.Context.SendActivityAsync(noIngredientsMsg, noIngredientsMsg, InputHints.IgnoringInput, cancellationToken);
				return await stepContext.ReplaceDialogAsync("WaterfallDialog", orderInfo);
			}
			var pizza = new Pizza { Ingredients = addedIngredients, Name = "Una pizza personalizada" };
			orderInfo.Pizzas.Add(pizza);

			var message = "He entendido que quieres una pizza de " +
				addedIngredients.Select(i => i.Name).ToArray().ConcatenateWith("y") + ".";
			var retryPrompt = "¿Perdona? No sé si eso es un sí o es un no. " +
				message;
			await _orderInfo.SetAsync(stepContext.Context, orderInfo);
			return await stepContext.PromptAsync("ConfirmIngredients", new PromptOptions
			{
				Prompt = MessageFactory.Text(message, message),
				RetryPrompt = MessageFactory.Text(retryPrompt, retryPrompt),
			}, cancellationToken);
		}

		private async Task<DialogTurnResult> AcceptIngredientsAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context);
			var confirm = (bool)stepContext.Result;
			if (!confirm)
			{
				string errorMsg = "Vaya, debo haber entendido algo mal. " +
				"Si quieres saber qué ingredientes puedes añadir a la pizza, di: \"ingredientes\". " +
				"Si quieres información sobre un ingrediente concreto, di: \"info de <ingrediente>\".";
				await stepContext.Context.SendActivityAsync(errorMsg, errorMsg, InputHints.IgnoringInput, cancellationToken);
				orderInfo.Pizzas.Remove(orderInfo.Pizzas[^1]);
				await _orderInfo.SetAsync(stepContext.Context, orderInfo);

				return await stepContext.ReplaceDialogAsync("WaterfallDialog", orderInfo);
			}
			return await stepContext.NextAsync();
		}
	}
}