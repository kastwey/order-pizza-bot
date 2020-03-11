using OrderPizzaBot.Entities;

using System.Collections.Generic;

namespace OrderPizzaBot.Contracts.Repositories
{
	public interface IPizzaRepository
	{

		IEnumerable<Pizza> GetPizzas();

		Pizza GetByName(string name);
	}
}
