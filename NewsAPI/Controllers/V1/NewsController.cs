using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Business.V1;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using Newtonsoft.Json;

namespace NewsAPI.Controllers.V1
{
    [ApiController]
    [Produces("application/json")]
    public class NewsController : ControllerBase
    {
        private readonly INewsHandler _newsService;

        public NewsController(INewsHandler newsService)
        {
            _newsService = newsService;
        }

        [HttpGet(Constant.ApiRoutes.News.GetNewsByFilter)]
        public async Task<PaginatedList<NewsResponse>> GetNewsByFilter(string filter = "{}")
        {
            var filterConvert = JsonConvert.DeserializeObject<NewsQueryFilter>(filter);

            return await _newsService.GetNewsByFilterAsync(filterConvert);
        }

        [HttpGet(Constant.ApiRoutes.News.GetNewsById)]
        public async Task<Response<NewsResponse>> GetNewsById([FromRoute] Guid newsId)
        {
            return await _newsService.GetNewsByIdAsync(newsId);
        }

        [HttpGet(Constant.ApiRoutes.News.GetNewsByCategoryId)]
        public async Task<Response<GetNewsByCategoryIdResponse>> GetNewsByCategoryId([FromRoute] Guid categoryId)
        {
            return await _newsService.GetNewsByCategoryId(categoryId);
        }

        [HttpPost(Constant.ApiRoutes.News.CreateNews)]
        public async Task<Response<NewsResponse>> CreateNews([FromBody] CreateNewsRequest request)
        {
            return await _newsService.CreateNewsAsync(request);
        }

        [HttpPut(Constant.ApiRoutes.News.UpdateNews)]
        public async Task<Response<NewsResponse>> UpdateNews([FromRoute] Guid newsId, [FromBody] UpdateNewsRequest request)
        {
            return await _newsService.UpdateNewsAsync(newsId, request);
        }

        [HttpDelete(Constant.ApiRoutes.News.DeleteNews)]
        public async Task<Response<NewsResponse>> DeleteNewsById([FromRoute] Guid newsId)
        {
            return await _newsService.DeleteNewsByIdAsync(newsId);
        }
    }
}