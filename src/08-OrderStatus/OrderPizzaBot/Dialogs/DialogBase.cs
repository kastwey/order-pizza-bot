using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

using System;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Dialogs
{
	public abstract class DialogBase : ComponentDialog
	{

		private readonly string HelpMsgText = "Para pedir una pizza, di algo como: \"Quiero pedir dos pizzas a domicilio.\"" + Environment.NewLine +
			"Si quieres conocer nuestra carta, di: \"dime la carta\"." + Environment.NewLine +
			"Si quieres salir del bot, di: \"cancelar\". " + Environment.NewLine +
			"Si deseas volver a escuchar el mensaje de ayuda, di: \"ayuda\".";
		
		private readonly string CancelMsgText = "Cancelando...";


		private readonly OrderPizzaRecognizer _recognizer;

		protected DialogBase(string id, OrderPizzaRecognizer recognizer)
			: base(id)
		{
			_recognizer = recognizer;
		}

		protected OrderPizzaRecognizer Recognizer => _recognizer;
		protected override async Task<DialogTurnResult> OnContinueDialogAsync(DialogContext innerDc, CancellationToken cancellationToken = default)
		{
			var result = await InterruptAsync(innerDc, cancellationToken);
			if (result != null)
			{
				return result;
			}

			return await base.OnContinueDialogAsync(innerDc, cancellationToken);
		}

		private async Task<DialogTurnResult> InterruptAsync(DialogContext innerDc, CancellationToken cancellationToken)
		{
			if (innerDc.Context.Activity.Type == ActivityTypes.Message)
			{
				var text = innerDc.Context.Activity.Text.ToLowerInvariant();

				switch (text)
				{
					case "ayuda":
					case "?":
						var helpMessage = MessageFactory.Text(HelpMsgText, HelpMsgText, InputHints.ExpectingInput);
						await innerDc.Context.SendActivityAsync(helpMessage, cancellationToken);
						await innerDc.RepromptDialogAsync(cancellationToken);
						return new DialogTurnResult(DialogTurnStatus.Waiting);
					case "cancelar":
					case "salir":
						var cancelMessage = MessageFactory.Text(CancelMsgText, CancelMsgText, InputHints.IgnoringInput);
						await innerDc.Context.SendActivityAsync(cancelMessage, cancellationToken);
						return await innerDc.CancelAllDialogsAsync(cancellationToken);
				}
			}

			return null;
		}

		protected async Task<bool> ValidateConfirmation(PromptValidatorContext<bool> validatorContext, CancellationToken cancellationToken)
		{
			var result = await _recognizer.RecognizeAsync<OrderPizza>(validatorContext.Context, cancellationToken);
			var intent = result.TopIntent();
			if (intent.intent == OrderPizza.Intent.Accept || intent.intent == OrderPizza.Intent.Reject)
			{
				validatorContext.Recognized.Succeeded = true;
				validatorContext.Recognized.Value = intent.intent == OrderPizza.Intent.Accept;
			}
			return validatorContext.Recognized.Succeeded;
		}


	}
}