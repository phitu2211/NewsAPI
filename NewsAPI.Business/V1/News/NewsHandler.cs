using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using NewsAPI.Data;
using NewsAPI.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NewsAPI.Business.V1
{
    public class NewsHandler : INewsHandler
    {
        private readonly NewsContext _newsContext;
        private readonly ILogger<NewsHandler> _logger;
        public NewsHandler(NewsContext newsContext, ILogger<NewsHandler> logger)
        {
            _logger = logger;
            _newsContext = newsContext;
        }

        #region CRUD
        /// <summary>
        /// Tạo mới tin tức
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<NewsResponse>> CreateNewsAsync(CreateNewsRequest request)
        {
            if (string.IsNullOrEmpty(request.Content) || string.IsNullOrEmpty(request.Title))
            {
                _logger.LogError("Title or content is not null");
                return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Title or content is not null" });
            }
            var news = new News
            { 
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                CreateTime = DateTime.UtcNow.Date,
                UpdateTime = DateTime.UtcNow.Date
            };

            var categoryNews = new List<CategoryNews>();

            if (request.CategoryIds != null && request.CategoryIds.Any() )
                foreach (var categoryId in request.CategoryIds)
                {
                    var category = await _newsContext.Categories.FindAsync(categoryId);
                    if (category == null)
                    {
                        _logger.LogError("Not find category");
                        return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Not find category" });
                    }
                    categoryNews.Add(new CategoryNews
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = category.Id,
                        NewsId = news.Id
                    });
                }

            await _newsContext.News.AddAsync(news);

            _newsContext.CategoryNews.AddRange(categoryNews);

            var result = await _newsContext.SaveChangesAsync();

            var dataReponse = new NewsResponse
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CreateTime = news.CreateTime,
                UpdateTime = news.UpdateTime,
                Categories = await GetCategoryModels(news.Id)
            };

            if (result > 0)
            {
                _logger.LogInformation("Create news success");
                return new Response<NewsResponse>(Constant.STATUS_SUCESS, null, dataReponse, 1);
            }

            _logger.LogError("Create news failed");
            return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Error when Save" });
        }

        /// <summary>
        /// Xóa tin tức
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<Response<NewsResponse>> DeleteNewsByIdAsync(Guid newsId)
        {
            var news = await _newsContext.News.FindAsync(newsId);

            if (news == null)
            {
                _logger.LogError("Not find news with id");
                return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Not find news with id" });
            }
            var categoryNews = _newsContext.CategoryNews.Where(x => x.NewsId.Equals(news.Id));

            if(categoryNews != null && categoryNews.Any())
                _newsContext.CategoryNews.RemoveRange(categoryNews);

            _newsContext.News.Remove(news);

            var result = await _newsContext.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Delete news success");
                return new Response<NewsResponse>(Constant.STATUS_SUCESS);
            }

            _logger.LogError("Delete news failed");
            return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Error Save" });
        }
        
        /// <summary>
        /// Xem danh sách tin tức theo filter: title, content, ...
        /// Phân trang
        /// </summary>
        /// <param name="filter"></param>
        /// <returns></returns>
        public async Task<PaginatedList<NewsResponse>> GetNewsByFilterAsync(NewsQueryFilter filter)
        {
            var dataResponse = new List<NewsResponse>();

            foreach (var item in _newsContext.News)
            {
                dataResponse.Add(new NewsResponse
                {
                    Id = item.Id,
                    Title = item.Title,
                    Content = item.Content,
                    CreateTime = item.CreateTime,
                    UpdateTime = item.UpdateTime,
                    Categories = await GetCategoryModels(item.Id)
                });
            }

            if (!string.IsNullOrEmpty(filter.Title))
                dataResponse = dataResponse.Where(x => x.Title.Contains(filter.Title)).ToList();

            if(!string.IsNullOrEmpty(filter.Content))
                dataResponse = dataResponse.Where(x => x.Content.Contains(filter.Content)).ToList();

            if (!filter.PageSize.HasValue && !filter.PageNumber.HasValue)
                return await PaginatedList<NewsResponse>.CreateAsync(dataResponse);

            return await PaginatedList<NewsResponse>.CreateAsync(dataResponse, filter.PageNumber.Value, filter.PageSize.Value);
        }

        /// <summary>
        /// Xem chi tiết tin tức theo id
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<Response<NewsResponse>> GetNewsByIdAsync(Guid newsId)
        {
            var news = await _newsContext.News.FindAsync(newsId);

            if (news == null)
            {
                _logger.LogError("Not find news with id");
                return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Not find news with id" });
            }

            var dataResponse = new NewsResponse
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CreateTime = news.CreateTime,
                UpdateTime = news.UpdateTime,
                Categories = await GetCategoryModels(news.Id)
            };

            return new Response<NewsResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Cập nhật tin tức, thêm xóa danh mục khỏi tin tức
        /// </summary>
        /// <param name="newsId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<NewsResponse>> UpdateNewsAsync(Guid newsId, UpdateNewsRequest request)
        {
            var news = await _newsContext.News.FindAsync(newsId);

            if (news == null)
            {
                _logger.LogError("Not find news with id");
                return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Not find news with id" });
            }
            if (!string.IsNullOrEmpty(request.Title))
                news.Title = request.Title;

            if (!string.IsNullOrEmpty(request.Content))
                news.Content = request.Content;

            //thêm danh mục vào tin tức
            foreach (var categoryId in request.AddCategoryIds ?? new List<Guid> { })
            {
                var category = await _newsContext.Categories.FindAsync(categoryId);
                if (category != null)
                {
                    var categoryNews = _newsContext.CategoryNews.Where(x => x.CategoryId.Equals(category.Id) && x.NewsId.Equals(news.Id)).ToList();

                    if (categoryNews.Count() <= 0)
                    {
                        await _newsContext.CategoryNews.AddAsync(new CategoryNews
                        {
                            Id = Guid.NewGuid(),
                            CategoryId = category.Id,
                            NewsId = news.Id
                        });
                    }
                }
            }

            //xóa danh mục khỏi tin tức
            foreach (var categoryId in request.RemoveCategoryIds ?? new List<Guid> { })
            {
                var category = await _newsContext.Categories.FindAsync(categoryId);
                if (category != null)
                {
                    var categoryNews = _newsContext.CategoryNews.Where(x => x.CategoryId.Equals(categoryId));
                    if (categoryNews != null && categoryNews.Any())
                        _newsContext.CategoryNews.RemoveRange(categoryNews);
                }
            }

            var result = await _newsContext.SaveChangesAsync();

            var dataResponse = new NewsResponse
            {
                Id = news.Id,
                Title = news.Title,
                Content = news.Content,
                CreateTime = news.CreateTime,
                UpdateTime = news.UpdateTime,
                Categories = await GetCategoryModels(news.Id)
            };

            if (result >= 0)
            {
                _logger.LogInformation("Update news sucess");
                return new Response<NewsResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogError("Update news failed");
            return new Response<NewsResponse>(Constant.STATUS_ERROR, new List<string> { "Error when save" });
        }

        #endregion

        /// <summary>
        /// Xem danh sách tin tức theo id danh mục
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<Response<GetNewsByCategoryIdResponse>> GetNewsByCategoryId(Guid categoryId)
        {
            var category = await _newsContext.Categories.FindAsync(categoryId);

            if (category == null)
            {
                _logger.LogError("Not find category with id");
                return new Response<GetNewsByCategoryIdResponse>(Constant.STATUS_ERROR, new List<string> { "No find category with id" });
            }

            var dataResponse = new GetNewsByCategoryIdResponse();
            //ds tin tức có trong danh mục
            var news = new List<NewsResponse>();
            //ds tin tức k có trong danh mục
            var noNews = new List<NewsResponse>();

            //thêm tin tức vào 2 danh sách trên
            foreach (var item in _newsContext.News)
            {
                var list = IsCategoryInNews(category, item) ? news : noNews;
                list.Add(new NewsResponse
                {
                    Id = item.Id,
                    Title = item.Title,
                    Content = item.Content,
                    CreateTime = item.CreateTime,
                    UpdateTime = item.UpdateTime,
                    Categories = await GetCategoryModels(item.Id)
                });
            }

            dataResponse.NewsBelongCategories = news;
            dataResponse.NewsNoBelongCategories = noNews;

            return new Response<GetNewsByCategoryIdResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Kiểm tra danh mục có tin tức k
        /// </summary>
        /// <param name="category"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        private bool IsCategoryInNews(Category category, News item)
        {
            var categoryNews = _newsContext.CategoryNews.Where(x => x.NewsId.Equals(item.Id) && x.CategoryId.Equals(category.Id));
            if (categoryNews != null && categoryNews.Any())
                return true;

            return false;
        }

        /// <summary>
        /// Lấy danh sách danh mục của tin tức
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        private async Task<List<CategoryResponse>> GetCategoryModels(Guid newsId)
        {
            var categories = new List<CategoryResponse>();

            var categoryNews = _newsContext.CategoryNews.Where(x => x.NewsId.Equals(newsId));

            if (categoryNews == null || !categoryNews.Any())
                return null;

            foreach (var categoryNew in categoryNews)
            {
                var category = await _newsContext.Categories.FindAsync(categoryNew.CategoryId);
                if (category != null)
                    categories.Add(new CategoryResponse
                    {
                        Id = category.Id,
                        Name = category.Name,
                        ParentId = category.ParentId
                    });
            }

            return categories;
        }
    }
}
