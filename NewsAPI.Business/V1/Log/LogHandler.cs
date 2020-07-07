using Nest;
using NewsAPI.Contracts.V1.Model;
using System.Threading.Tasks;
using NewsAPI.Contracts.V1.Helper;
using System.Linq;
using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace NewsAPI.Business.V1
{
    public class LogHandler : ILogHandler
    {
        private readonly IElasticClient _elasticClient;
        private readonly ILogger<LogHandler> _logger;

        public LogHandler(IElasticClient elasticClient, ILogger<LogHandler> logger)
        {
            _logger = logger;
            _elasticClient = elasticClient;
        }

        /// <summary>
        /// Xem danh sách log theo filter: level, message,..
        /// Phân trang
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PaginatedList<LogModel>> GetLogByFilterAsync(LogQueryFilter filter)
        {
            var dataResponse = (await _elasticClient.SearchAsync<LogModel>(s => s.MatchAll()))
                .Hits.Select(x => new LogModel 
                { 
                    Id = x.Id, 
                    Level = x.Source.Level,
                    Message = x.Source.Message 
                }).ToList();

            if (!string.IsNullOrEmpty(filter.Level))
                dataResponse = dataResponse.Where(x => x.Level.Equals(filter.Level)).ToList();

            if (!string.IsNullOrEmpty(filter.Message))
                dataResponse = dataResponse.Where(x => x.Message.Contains(filter.Message)).ToList();

            if (!filter.PageSize.HasValue && !filter.PageNumber.HasValue)
                return await PaginatedList<LogModel>.CreateAsync(dataResponse);

            return await PaginatedList<LogModel>.CreateAsync(dataResponse, filter.PageNumber.Value, filter.PageSize.Value);
        }

        /// <summary>
        /// Xem log theo id
        /// </summary>
        /// <param name="logId"></param>
        /// <returns></returns>
        public async Task<Response<LogModel>> GetLogByIdAsync(string logId)
        {
            var res = await _elasticClient.GetAsync<LogModel>(logId);

            if (!res.Found)
            {
                _logger.LogError("Not find log with id");
                return new Response<LogModel>(Constant.STATUS_ERROR, new List<string> { "Not find log with id" });
            }

            var dataResponse = new LogModel
            {
                Id = res.Source.Id,
                Level = res.Source.Level,
                Message = res.Source.Message
            };

            return new Response<LogModel>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }
    }
}
