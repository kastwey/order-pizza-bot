
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

using OrderPizzaBot.Dialogs;

namespace OrderPizzaBot
{
	public class Startup
	{
		public Startup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddMvc();

			// Create the Bot Framework Adapter with error handling enabled.
			services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
			// Create the storage we'll be using for User and Conversation state. (Memory is great for testing purposes.)
			services.AddSingleton<IStorage, MemoryStorage>();

			// Create the User state. (Used in this bot's Dialog implementation.)
			services.AddSingleton<UserState>();

			// Create the Conversation state. (Used by the Dialog system itself.)
			services.AddSingleton<ConversationState>();


			// Register LUIS recognizer
			services.AddSingleton<OrderPizzaRecognizer>();
			services.AddSingleton<MainDialog>();

			// Create the bot as a transient. In this case the ASP Controller is expecting an IBot.
			services.AddTransient<IBot, OrderPizzaBot.Bots.OrderPizzaBot>();

		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}
			else
			{
				app.UseHsts();
			}

			app.UseDefaultFiles();
			app.UseStaticFiles();
			app.UseWebSockets();
			app.UseRouting();
			//app.UseHttpsRedirection();
			app.UseEndpoints(opt =>
			{
				opt.MapDefaultControllerRoute();
			});
		}
	}
}
