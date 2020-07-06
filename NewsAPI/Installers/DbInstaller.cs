using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NewsAPI.Data;
using NewsAPI.Data.Context;
using NewsAPI.Extensions;
using System;
using NewsAPI.Business.V1;

namespace NewsAPI.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallerServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddElasticsearch(configuration);

            services.AddDbContext<NewsContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection")));
            services.AddIdentity<AppUser, AppRole>()
                .AddEntityFrameworkStores<NewsContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            services.AddScoped<ILogHandler, LogHandler>();
            services.AddScoped<IAccountHandler, AccountHandler>();
            services.AddScoped<IMenuHandler, MenuHandler>();
            services.AddScoped<IRoleHandler, RoleHandler>();
            services.AddScoped<INewsHandler, NewsHandler>();
            services.AddScoped<ICategoryHandler, CategoryHandler>();
        }
    }
}
