// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.6.2

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using OrderPizzaBot.Contracts.Repositories;
using OrderPizzaBot.Dialogs;

namespace OrderPizzaBot.Bots
{
	public class OrderPizzaBot : ActivityHandler
	{

		protected readonly Dialog Dialog;
		protected readonly BotState ConversationState;
		protected readonly BotState UserState;
		protected readonly ILogger Logger;

		private readonly OrderPizzaRecognizer _recognizer;

		private readonly IPizzaRepository _pizzaRepository;

		private readonly IIngredientRepository _ingredientRepository;

		public OrderPizzaBot(ConversationState conversationState, UserState userState, ILogger<OrderPizzaBot> logger, OrderPizzaRecognizer recognizer, IPizzaRepository pizzaRepository, IIngredientRepository ingredientRepository)
		{
			ConversationState = conversationState;
			UserState = userState;
			Logger = logger;
			_recognizer = recognizer;
			_pizzaRepository = pizzaRepository;
			_ingredientRepository = ingredientRepository;
			Dialog = new MainDialog(_recognizer, userState, pizzaRepository, ingredientRepository);
		}


		public override async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default)
		{
			await base.OnTurnAsync(turnContext, cancellationToken);

			// Save any state changes that might have occured during the turn.
			await ConversationState.SaveChangesAsync(turnContext, false, cancellationToken);
			await UserState.SaveChangesAsync(turnContext, false, cancellationToken);
		}

		protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
		{
			await Dialog.RunAsync(turnContext, ConversationState.CreateProperty<DialogState>(nameof(DialogState)), cancellationToken);
		}

		protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
		{
			var welcomeText = "Hello and welcome!";
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
