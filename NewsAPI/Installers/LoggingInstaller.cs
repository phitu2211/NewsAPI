using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Installers
{
    public class LoggingInstaller : IInstaller
    {
        public void InstallerServices(IServiceCollection services, IConfiguration configuration)
        {
			var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

			Serilog.Log.Logger = new LoggerConfiguration()
				.Enrich.FromLogContext()
				.Enrich.WithMachineName()
				.WriteTo.Debug()
				.WriteTo.Console()
				.WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri(configuration["ElasticSearchConfiguration:Uri"]))
				{
					AutoRegisterTemplate = true,
					IndexFormat = configuration["ElasticSearchConfiguration:DefaultIndex"]
				})
				.Enrich.WithProperty("Environment", environment)
				.ReadFrom.Configuration(configuration)
				.CreateLogger();
		}
    }
}
