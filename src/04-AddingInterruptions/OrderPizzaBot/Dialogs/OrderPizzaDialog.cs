using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Choices;
using Microsoft.Bot.Schema;

using OrderPizzaBot.Entities;
using OrderPizzaBot.Extensions;

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Dialogs
{
	public class OrderPizzaDialog : DialogBase
	{

		private readonly UserState _userState;

		private readonly IStatePropertyAccessor<OrderInfo> _orderInfo;

		public OrderPizzaDialog(UserState userState)
			: base(nameof(OrderPizzaDialog))
		{
			_userState = userState;
			_orderInfo = _userState.CreateProperty<OrderInfo>("OrderInfo");
			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new NumberPrompt<int>(nameof(NumberPrompt<int>), null, "es"));
			AddDialog(new ChoicePrompt(nameof(ChoicePrompt), null, "es"));
			AddDialog(new ConfirmPrompt(nameof(ConfirmPrompt), null, "es"));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				NumberOfPizzasAsync,
				OrderTypeAsync,
				ConfirmPizzaSelectionAsync,
				StartPizzaConfigurationAsync,
			}));
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> NumberOfPizzasAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context);
			if (!orderInfo.NumberOfPizzas.HasValue)
			{
				var msg = "¿Cuántas pizzas quieres?";
				var promptMessage = MessageFactory.Text(msg, msg, InputHints.ExpectingInput);
				return await stepContext.PromptAsync(nameof(NumberPrompt<int>),
					new PromptOptions
					{
						Prompt = promptMessage,
						RetryPrompt = MessageFactory.Text("Por favor, escribe un número del uno al diez. ¿Cuántas pizzas quieres?")
					}, cancellationToken);
			}
			return await stepContext.NextAsync();
		}


		private async Task<DialogTurnResult> OrderTypeAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context);
			if (!orderInfo.NumberOfPizzas.HasValue)
			{
				orderInfo.NumberOfPizzas = (int)stepContext.Result;
			}
			if (orderInfo.OrderType == OrderType.Undefined)
			{
				var message = "¿Quieres tu pedido a domicilio o para recoger?";
				return await stepContext.PromptAsync(nameof(ChoicePrompt),
				new PromptOptions
				{
					Choices = ChoiceFactory.ToChoices(new List<string> { "a domicilio", "para recoger" }),
					Prompt = MessageFactory.Text(message, message, InputHints.ExpectingInput),
					RetryPrompt = MessageFactory.Text("Perdona, no entendí tu respuesta. " + message)
				}, cancellationToken);
			}
			return await stepContext.NextAsync();
		}

		private async Task<DialogTurnResult> ConfirmPizzaSelectionAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var orderInfo = await _orderInfo.GetAsync(stepContext.Context, null, cancellationToken);
			if (orderInfo.OrderType == OrderType.Undefined)
			{
				int choiceIndex = ((FoundChoice)stepContext.Result).Index + 1;
				orderInfo.OrderType = (OrderType)choiceIndex;
			}
			await _orderInfo.SetAsync(stepContext.Context, orderInfo, cancellationToken);
			string plural = orderInfo.NumberOfPizzas > 1 ? "s" : string.Empty;
			string message = $"¡Perfecto! He entendido que quieres {orderInfo.NumberOfPizzas} pizza{plural} " +
				
				$"{orderInfo.OrderType.GetDescription()}. ¿Es correcto?";
			return await stepContext.PromptAsync(nameof(ConfirmPrompt), new PromptOptions { Prompt = MessageFactory.Text(message, message, InputHints.ExpectingInput) });
		}

		private async Task<DialogTurnResult> StartPizzaConfigurationAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			bool right = (bool)stepContext.Result;
			if (!right)
			{
				string sorryMessage = "¡Oh, vaya, lo siento, te habré entendido mal! ¡Empecemos de nuevo!";
				await stepContext.Context.SendActivityAsync(sorryMessage, sorryMessage, InputHints.IgnoringInput, cancellationToken);
				await _orderInfo.SetAsync(stepContext.Context, new OrderInfo());
				return await stepContext.ReplaceDialogAsync(nameof(WaterfallDialog), null, cancellationToken);
			}

			var endMsg = "¡Estupendo! ¡Por ahora esto es todo! ¡Vamos al siguiente paso del taller! ¡Pero eh! ¡Ya tenemos el número de pizzas y el tipo de pedido! ¡Y además, ya sabemso cómo salir del diálogo o pedir ayuda en cualquier momento!";
			await stepContext.Context.SendActivityAsync(endMsg, endMsg, InputHints.IgnoringInput);
			return await stepContext.NextAsync();
		}
	}
}
