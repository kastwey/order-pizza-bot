// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using OrderPizzaBot.Contracts.Repositories;
using OrderPizzaBot.Dialogs;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Bots
{
	public class OrderPizzaBot : ActivityHandler
	{

		private readonly Dialog _dialog;
		private readonly BotState _conversationState;
		private readonly BotState _userState;
		private readonly OrderPizzaRecognizer _recognizer;
		private readonly IPizzaRepository _pizzaRepository;

		public OrderPizzaBot(ConversationState conversationState, UserState userState, OrderPizzaRecognizer recognizer, IPizzaRepository pizzaRepository)
		{
			_conversationState = conversationState;
			_userState = userState;
			_recognizer = recognizer;
			_pizzaRepository = pizzaRepository;
			_dialog = new MainDialog(_recognizer, userState, _pizzaRepository);
		}


		public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
		{
			await base.OnTurnAsync(turnContext, cancellationToken);

			// Save any state changes that might have occured during the turn.
			await _conversationState.SaveChangesAsync(turnContext, false, cancellationToken);
			await _userState.SaveChangesAsync(turnContext, false, cancellationToken);
		}

		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			await _dialog.RunAsync(turnContext, _conversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
		}

		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
		{
			var welcomeText = "Bienvenido a este bot súper inteligente para pedir pizzas. ¡Encantado de saludarte! ¿En qué te puedo ayudar?";
			foreach (var member in membersAdded)
			{
				if (member.Id != turnContext.Activity.Recipient.Id)
				{
					await turnContext.SendActivityAsync(MessageFactory.Text(welcomeText, welcomeText), cancellationToken);
				}
			}
		}
	}
}
