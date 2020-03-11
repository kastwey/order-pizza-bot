using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

using OrderPizzaBot.Constants;
using OrderPizzaBot.Contracts.Repositories;
using OrderPizzaBot.Entities;
using OrderPizzaBot.Helpers;

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Dialogs
{

	public sealed class MainDialog : DialogBase
	{
		private readonly OrderPizzaRecognizer _recognizer;

		private readonly UserState _userState;

		private readonly IPizzaRepository _pizzaRepository;

		private readonly IIngredientRepository _ingredientRepository;

		private readonly IStatePropertyAccessor<OrderInfo> _orderInfo;


		public MainDialog(OrderPizzaRecognizer recognizer, UserState userState, IPizzaRepository pizzaRepository, IIngredientRepository ingredientRepository)
			: base(nameof(MainDialog), userState, recognizer)
		{
			_recognizer = recognizer;
			_userState = userState;
			_pizzaRepository = pizzaRepository;
		 	_ingredientRepository = ingredientRepository;
			_orderInfo = _userState.CreateProperty<OrderInfo>("OrderInfo");

			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new OrderPizzaDialog(userState, recognizer, pizzaRepository, ingredientRepository));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
				{
					ActAsync
				}));
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> ActAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			string replyText = string.Empty;
			OrderInfo orderInfo;

			var result = await _recognizer.RecognizeAsync<OrderPizza>(stepContext.Context, cancellationToken);
			var intent = result.TopIntent();
			switch (intent.intent)
			{
				case OrderPizza.Intent.Greet:
					replyText = "¡Hola! ¡Bienvenido a este bot súper inteligente para pedir pizzas. ¿Qué quieres hacer: pedir una pizza, conocer nuestra carta o salir?";
					break;
				case OrderPizza.Intent.OrderPizza:
					orderInfo = await _orderInfo.GetAsync(stepContext.Context);
					if (orderInfo != null)
					{
						if ((DateTime.Now - orderInfo.OrderDate).TotalMinutes > 30)
						{
							await _orderInfo.DeleteAsync(stepContext.Context, cancellationToken);
							orderInfo = null;
						}
						else
						{
							replyText = "Lo siento, tienes un pedido en curso. NO podrás pedir nada más hasta que tu pedido actual esté finalizado.";
						}
					}

					if (orderInfo == null)
					{
						return await StartOrderingPizzaAsync(stepContext, result, cancellationToken);
					}
					break;
				case OrderPizza.Intent.OrderStatus:
					orderInfo = await _orderInfo.GetAsync(stepContext.Context);
					if (orderInfo == null)
					{
						replyText = "Aún no has hecho ningún pedido. Si quieres, podemos empezar. Dime qué deseas.";
					}
					else
					{
						int timeElapsed = Convert.ToInt32((DateTime.Now - orderInfo.OrderDate).TotalMinutes);
						var entry = (orderInfo.OrderType == OrderType.Delivery ?  BotConstants.DeliveryStateMessages : BotConstants.PickupStateMessages)
							.FirstOrDefault(e => e.Key >= timeElapsed);
						if (entry.Value != null)
						{
							replyText = entry.Value;
						}
						else
						{
							replyText = "¡Ya tienes tu pedido (a no ser que el repartidor se lo haya comido, claro). ¡Que aproveche!";
						}
					}
					break;
				case OrderPizza.Intent.GetMenu:
					await MenuHelper.PrintMenuAsync(stepContext, _pizzaRepository, cancellationToken);
					return new DialogTurnResult(DialogTurnStatus.Waiting);
				default:
					replyText = "¿Qué? Aún soy un poco tontorón así que intenta afinar más con tus órdenes. ¡Gracias! ;)";
					break;
			}
			await SendMessageAsync(stepContext.Context, replyText, cancellationToken);
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
			return await stepContext.BeginDialogAsync(nameof(OrderPizzaDialog), orderInfo, cancellationToken);
		}
	}
}
