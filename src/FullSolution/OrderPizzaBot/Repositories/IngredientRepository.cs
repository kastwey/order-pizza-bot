using Newtonsoft.Json;

using OrderPizzaBot.Contracts.Repositories;
using OrderPizzaBot.Entities;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OrderPizzaBot.Repositories
{
	public class IngredientRepository : IIngredientRepository
	{
		private readonly  List<Ingredient> _ingredients;

		public IngredientRepository()
		{
			_ingredients = JsonConvert.DeserializeObject<List<Ingredient>>(File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "files", "ingredients.json")));
		}

		public IEnumerable<Ingredient> GetIngredients() => _ingredients;

		public Ingredient GetIngredientByName(string name) => _ingredients.SingleOrDefault(i => i.Name == name);

		public Ingredient GetIngredientById(int id) => _ingredients.SingleOrDefault(i => i.Id == id);

	}
}
