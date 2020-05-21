using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using OrderPizzaBot.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace OrderPizzaBot.Helpers
{
	public static class MenuHelper
	{

		public static async Task PrintMenuAsync(WaterfallStepContext stepContext, IPizzaRepository pizzaRepository, CancellationToken cancellationToken)
		{
			var pizzas = pizzaRepository.GetPizzas();
			var menu = new StringBuilder();
			menu.AppendLine("¡Nuestra carta!");
			foreach (var pizza in pizzas)
			{
				menu.AppendLine($"- {pizza.Name}: {(String.Join(", ", pizza.Ingredients.Select(i => i.Name)))}.");
					}
			var menuStr = menu.ToString();
			await stepContext.Context.SendActivityAsync(menuStr, menuStr, InputHints.IgnoringInput, cancellationToken);
		}


	}
}
