using Newtonsoft.Json;

using OrderPizzaBot.Contracts.Repositories;
using OrderPizzaBot.Entities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrderPizzaBot.Repositories
{
	public class PizzaRepository : IPizzaRepository
	{

		private readonly List<Pizza> _predefinedPizzas;

		public IEnumerable<Pizza> GetPizzas() => _predefinedPizzas;

		public Pizza GetByName(string name) => _predefinedPizzas.SingleOrDefault(p => p.Name == name);

		public PizzaRepository()
		{
			_predefinedPizzas = JsonConvert.DeserializeObject<List<Pizza>>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "files", "pizzas.json")));
		}
	}
}
