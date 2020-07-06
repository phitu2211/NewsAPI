using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Contracts.V1.Helper;
using Newtonsoft.Json;
using NewsAPI.Contracts.V1.Model;
using System;
using NewsAPI.Business.V1;

namespace NewsAPI.Controllers.V1
{
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogHandler _logService;

        public LogController(ILogHandler logService)
        {
            _logService = logService;
        }

        [HttpGet(Constant.ApiRoutes.Log.GetLogByFilter)]
        public async Task<PaginatedList<LogModel>> GetLogByFilter(string filter = "{}")
        {
            var filterConvert = JsonConvert.DeserializeObject<LogQueryFilter>(filter);

            return await _logService.GetLogByFilterAsync(filterConvert);
        }

        [HttpGet(Constant.ApiRoutes.Log.GetLogById)]
        public async Task<Response<LogModel>> GetLogById(Guid logId)
        {
            return await _logService.GetLogByIdAsync(logId);
        }
    }
}