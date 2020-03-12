// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

using OrderPizzaBot.Dialogs;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Bots
{
	public class OrderPizzaBot : ActivityHandler
	{

		private readonly BotState _conversationState;

		private readonly Dialog _dialog;

		private readonly OrderPizzaRecognizer _recognizer;

		public OrderPizzaBot(ConversationState conversationState, OrderPizzaRecognizer recognizer)
		{
			_conversationState = conversationState;
			_recognizer = recognizer;
			_dialog = new MainDialog(_recognizer);
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
