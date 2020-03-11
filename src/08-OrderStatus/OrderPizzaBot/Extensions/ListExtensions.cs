using System;
using System.Linq;

namespace OrderPizzaBot.Extensions
{
	public static class ListExtensions
	{

		public static string ConcatenateWith(this string[]  items, string conjuntion)
		{
			if (items is null)
			{
				throw new ArgumentNullException(nameof(items));
			}
			return String.Join(", ", items[0..^1]) +
				(items.Length > 1 ? $" {conjuntion} " : string.Empty) +
				items.Last(); 
		}
		}
}
