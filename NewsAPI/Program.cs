using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Data;
using NewsAPI.Data.Context;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace NewsAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
			try
			{
				var host = CreateHostBuilder(args).Build();

				using (var serviceScope = host.Services.CreateScope())
				{
					var dbContext = serviceScope.ServiceProvider.GetRequiredService<NewsContext>();

					await dbContext.Database.MigrateAsync();

					var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<AppRole>>();

					if (!await roleManager.RoleExistsAsync(Constant.ROLE_ADMIN))
					{
						var adminRole = new AppRole
						{ 
							Name = "Admin",
							Description = "Do Anything"
						};
						await roleManager.CreateAsync(adminRole);
					}

					if (!await roleManager.RoleExistsAsync(Constant.ROLE_USER))
					{
						var userRole = new AppRole
						{
							Name = "User",
							Description = "View News"
						};
						await roleManager.CreateAsync(userRole);
					}
				}

				await host.RunAsync();
			}
			catch (Exception ex)
			{
				Serilog.Log.Fatal($"Failed to start {Assembly.GetExecutingAssembly().GetName().Name}", ex);
				throw;
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
				.ConfigureAppConfiguration(configuration =>
				{
					configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
					configuration.AddJsonFile(
						$"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json",
						optional: true);
				})
				.UseSerilog();
    }
}
