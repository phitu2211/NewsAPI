using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using System;
using System.Threading.Tasks;

namespace NewsAPI.Business.V1
{
    public interface ILogHandler
    {
        Task<PaginatedList<LogModel>> GetLogByFilterAsync(LogQueryFilter filter);
        Task<Response<LogModel>> GetLogByIdAsync(string logId);
    }
}
