

using frznUpload.Web.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.ServiceProcess;

namespace frznUpload.Web
{
	public class Program
	{
		public static void Main(string[] args)
		{
			IHost host = CreateHostBuilder(args).Build();

			using (IServiceScope scope = host.Services.CreateScope())
			{
				using (Database database = scope.ServiceProvider.GetService<Database>())
					database.Database.Migrate();

				ILogger<Program> logger = scope.ServiceProvider.GetService<ILogger<Program>>();
				IConfiguration config = scope.ServiceProvider.GetService<IConfiguration>();

				logger.LogInformation(((IConfigurationRoot)config).GetDebugView());
			}

			host.Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
			.ConfigureAppConfiguration((hostingContext, config) =>
			{
				IHostEnvironment env = hostingContext.HostingEnvironment;

				config.AddJsonFile("appsettings.json", optional: true)
					.AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
					.AddJsonFile("/config/appsettings.json", optional: true);

				config.AddEnvironmentVariables();
			})
			.ConfigureWebHostDefaults(webBuilder =>
			{
				webBuilder.ConfigureKestrel((context, options) =>
				{
					// Handle requests up to 50 MB
					options.Limits.MaxRequestBodySize = int.MaxValue;
				}).UseStartup<Startup>();
			});
	}
}
