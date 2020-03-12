using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Dialogs
{

	public sealed class MainDialog : ComponentDialog
	{
		private readonly OrderPizzaRecognizer _recognizer;

		
		public MainDialog(OrderPizzaRecognizer recognizer)
			: base(nameof(MainDialog))
		{
			_recognizer = recognizer;
			AddDialog(new TextPrompt(nameof(TextPrompt)));
			AddDialog(new WaterfallDialog(nameof(WaterfallDialog), new WaterfallStep[]
			{
				ActAsync
			}));
			InitialDialogId = nameof(WaterfallDialog);
		}

		private async Task<DialogTurnResult> ActAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
		{
			var result = await _recognizer.RecognizeAsync<OrderPizza>(stepContext.Context, cancellationToken);
			var intent = result.TopIntent();
			var message = $"He entendido que quieres {intent.intent}, con una confianza de {intent.score}.";
			await stepContext.Context.SendActivityAsync(message, message, InputHints.IgnoringInput);
			return await stepContext.NextAsync();
		}

		
	}
}
