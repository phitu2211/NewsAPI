using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Installers
{
    public class SwaggerInstaller : IInstaller
    {
        public void InstallerServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo { Title = "News API", Version = "v1" });

				var security = new OpenApiSecurityRequirement();
				security.Add(new OpenApiSecurityScheme
				{
					Reference = new OpenApiReference
					{
						Type = ReferenceType.SecurityScheme,
						Id = "Bearer"
					},
					Scheme = "Bearer",
					Name = "Bearer",
					In = ParameterLocation.Header
				}, new List<string>());
				x.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
				{
					Scheme = "Bearer",
					Description = "JWT Authorization header using the bearer scheme",
					Name = "Authorization",
					In = ParameterLocation.Header,
					Type = SecuritySchemeType.ApiKey
				});
				x.AddSecurityRequirement(security);
			});
        }
    }
}
