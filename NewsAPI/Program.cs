using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Data;
using NewsAPI.Data.Context;
using Serilog;

namespace NewsAPI
{
    public class Program
    {
        public static async Task Main(string[] args)
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

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                }).UseSerilog();
    }
}
