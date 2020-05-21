namespace OrderPizzaBot.Entities
{
	public class Ingredient
	{

		public Ingredient(int id, string name, string description, double price)
		{
			Id = id;
			Name = name;
			Description = description;
			Price = price;
		}

		public int Id { get; set; }

		public string Name { get; set; }

		public string Description { get; set; }

		public double Price { get; set; }

	}
}
