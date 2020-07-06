using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsAPI.Business.V1
{
    public interface INewsHandler
    {
        Task<PaginatedList<NewsResponse>> GetNewsByFilterAsync(NewsQueryFilter filter);
        Task<Response<NewsResponse>> GetNewsByIdAsync(Guid newsId);
        Task<Response<GetNewsByCategoryIdResponse>> GetNewsByCategoryId(Guid categoryId);
        Task<Response<NewsResponse>> CreateNewsAsync(CreateNewsRequest request);
        Task<Response<NewsResponse>> DeleteNewsByIdAsync(Guid newsId);
        Task<Response<NewsResponse>> UpdateNewsAsync(Guid newsId, UpdateNewsRequest request);
    }
}
