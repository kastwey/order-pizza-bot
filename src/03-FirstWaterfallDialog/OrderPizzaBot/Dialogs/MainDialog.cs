using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using OrderPizzaBot.Entities;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Dialogs
{

	public sealed class MainDialog : ComponentDialog
	{
		private readonly OrderPizzaRecognizer _recognizer;

		private readonly UserState _userState;

		private readonly IStatePropertyAccessor<OrderInfo> _orderInfo;

		public MainDialog(OrderPizzaRecognizer recognizer, UserState userState)
			: base(nameof(MainDialog))
		{
			_recognizer = recognizer;
			_userState = userState;
			_orderInfo = _userState.CreateProperty<OrderInfo>("OrderInfo");

			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new OrderPizzaDialog(userState));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
				{
					ActAsync
				}));
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> ActAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			string replyText;

			var result = await _recognizer.RecognizeAsync<OrderPizza>(stepContext.Context, cancellationToken);
			var intent = result.TopIntent();
			switch (intent.intent)
			{
				case OrderPizza.Intent.Greet:
					replyText = "¡Hola! ¡Bienvenido a este bot súper inteligente para pedir pizzas. ¿Qué quieres hacer: pedir una pizza, conocer nuestra carta o salir?";
					break;
				case OrderPizza.Intent.OrderPizza:
					return await StartOrderingPizzaAsync(stepContext, result, cancellationToken);
				case OrderPizza.Intent.OrderStatus:
					replyText = "Aún no está esto implementado. Espera un poco, impaciente. ;)";
					break;
				case OrderPizza.Intent.GetMenu:
					replyText = "Aún no está esto implementado. Espera un poco, impaciente. ;)";
					break;
				default:
					replyText = "¿Qué? Aún soy un poco tontorón así que intenta afinar más con tus órdenes. ¡Gracias! ;)";
					break;
			}
			await stepContext.Context.SendActivityAsync(replyText, replyText, InputHints.ExpectingInput, cancellationToken);
			return await stepContext.NextAsync();
		}

		
		private async Task<DialogTurnResult> StartOrderingPizzaAsync(WaterfallStepContext stepContext, OrderPizza result, CancellationToken cancellationToken)
		{
			var orderInfo = new OrderInfo();
			await _orderInfo.SetAsync(stepContext.Context, orderInfo, cancellationToken);
			var entities = result.Entities;
			double? number = entities.number?.FirstOrDefault();
			var orderType = entities.OrderType?.FirstOrDefault()?.FirstOrDefault();
			orderInfo.OrderType = orderType switch
			{
				"Delivery" => OrderType.Delivery,
				"PickUp" => OrderType.PickUp,
				_ => OrderType.Undefined
			};

			if (number.HasValue)
			{
				orderInfo.NumberOfPizzas = Convert.ToInt32(number.Value);
			}
			return await stepContext.BeginDialogAsync(nameof(OrderPizzaDialog), null, cancellationToken);
		}
	}
}
