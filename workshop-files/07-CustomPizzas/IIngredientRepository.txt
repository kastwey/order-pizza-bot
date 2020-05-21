using OrderPizzaBot.Entities;

using System.Collections.Generic;

namespace OrderPizzaBot.Contracts.Repositories
{
	public interface IIngredientRepository
	{

		IEnumerable<Ingredient> GetIngredients();

		Ingredient GetIngredientByName(string name);

		Ingredient GetIngredientById(int id);

	}
}
