using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Nest;
using NewsAPI.Contracts.V1.Model;
using NewsAPI.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Extensions
{
    public static class ElasticsearchExtension
    {
        public static void AddElasticsearch(
              this IServiceCollection services, IConfiguration configuration)
        {
            var elasticSearchConfiguration = new ElasticSearchConfiguration();
            configuration.GetSection(nameof(ElasticSearchConfiguration)).Bind(elasticSearchConfiguration);

            var settings = new ConnectionSettings(new Uri(elasticSearchConfiguration.Uri))
                .DefaultIndex(elasticSearchConfiguration.DefaultIndex)
                .DefaultMappingFor<LogModel>(map => map.IndexName(elasticSearchConfiguration.DefaultIndex));

            var client = new ElasticClient(settings);

            services.AddSingleton<IElasticClient>(client);
            services.AddSingleton(elasticSearchConfiguration);
        }
    }
}
