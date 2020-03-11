using System.Collections.Generic;

namespace OrderPizzaBot.Constants
{
	public static class BotConstants
	{

		public const int MaxPromptAttempts = 3;

		public static readonly Dictionary<int, string> DeliveryStateMessages = new Dictionary<int, string>
		{
			{0, "¡Tu pedido aún está en la cola! ¡Espera un poco!" },
			{  1, "¡Nuestros cocineros ya están preparando tu pedido!" },
			{ 7,  "¡Tu pedido está en el horno!" },
			{ 16,  "¡Esperando a que el repartidor recoja tu pedido!"  },
			{ 20,  "¡El repartidor ya ha recogido tu pedido y va de camino!" },
			{ 25,  "¡El repartidor está llegando! ¡Pon la mesa!" },
		};

		public static readonly Dictionary<int, string> PickupStateMessages = new Dictionary<int, string>
		{
			{0, "¡Tu pedido aún está en la cola! ¡Espera un poco!" },
			{  1, "¡Nuestros cocineros ya están preparando tu pedido!" },
			{ 7,  "¡Tu pedido está en el horno!" },
			{ 16,  "¡Tu pedido está listo! ¡Puedes venir a recogerlo cuando quieras!"  },
			{ 20,  "¡Tu pedido ya está listo y se está enfriando! ¡Corre!" },
			{ 25,  "¡Tu pedido lleva un ratazo preparado y se está quedando helado!" },
		};

	}
}
