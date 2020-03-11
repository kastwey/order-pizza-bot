using System;
using System.Collections.Generic;

namespace OrderPizzaBot.Entities
{
	public class OrderInfo
	{
		
		public OrderType OrderType { get; set; }

		public List<Pizza> Pizzas { get; set; } = new List<Pizza>();

		public int? NumberOfPizzas { get; set; }

		public int ConfiguringPizzaIndex { get; set; }
		
		public DateTime OrderDate { get; set; }
	}
}
