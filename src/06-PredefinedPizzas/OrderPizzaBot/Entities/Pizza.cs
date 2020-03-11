using OrderPizzaBot.Extensions;
using System.Linq;
using System.Collections.Generic;

namespace OrderPizzaBot.Entities
{
	public class Pizza
	{

		public PizzaSize Size { get; set; } = PizzaSize.Medium;

		public DoughType Dough { get; set; } = DoughType.Regular;

		public string Name { get; set; }

		public List<Ingredient> Ingredients { get; set; } = new List<Ingredient>();


		public Pizza(string name, List<Ingredient> ingredients, PizzaSize size = PizzaSize.Undefined, DoughType dough = DoughType.Undefined)
		{
			Name = name;
			Ingredients = ingredients;
			Size = size;
			Dough = dough;
		}

		public Pizza()
		{
		}

		public override string ToString()
		{
			return $"Pizza {Name} ( {this.Ingredients.Select(i => i.Name).ToArray().ConcatenateWith("y")}), {Size.GetDescription()}, masa {Dough.GetDescription()}";
		}
	}
}
