using OrderPizzaBot.Entities;

using System.Linq;

namespace OrderPizzaBot.Extensions
{
	public static class PizzaExtensions
	{

		public static double GetTotalPrice(this Pizza pizza) => (3 * (int)pizza.Size) + pizza.Ingredients.Sum(i => i.Price);

	}
}
