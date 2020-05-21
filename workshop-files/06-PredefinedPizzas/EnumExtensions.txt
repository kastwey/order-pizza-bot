using OrderPizzaBot.Entities;

namespace OrderPizzaBot.Extensions
{
	public static class EnumExtensions
	{

		public static string GetDescription(this PizzaSize pizzaSize)
		{
			return pizzaSize switch
			{
				PizzaSize.Small => "pequeña",
				PizzaSize.Medium => "mediana",
				PizzaSize.Large => "grande",
				PizzaSize.Undefined => string.Empty,
				_ => "desconocida"
			};
		}

		public static string GetDescription(this OrderType orderType)
		{
			return orderType switch
			{
				OrderType.PickUp => "a recoger",
				OrderType.Delivery => "a domicilio",
				OrderType.Undefined => string.Empty,
				_ => "desconocido"
			};
		}

		public static string GetDescription(this DoughType doughType)
		{
			return doughType switch
			{
				DoughType.Thin => "fina",
				DoughType.Regular => "normal",
				DoughType.Undefined => string.Empty,
				_ => "desconocida"
			};
		}
	}
}
