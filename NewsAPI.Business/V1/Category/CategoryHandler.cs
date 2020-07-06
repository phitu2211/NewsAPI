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
    public class CategoryHandler : ICategoryHandler
    {
        private readonly NewsContext _newsContext;
        private readonly ILogger<CategoryHandler> _logger;

        public CategoryHandler(NewsContext newsContext, ILogger<CategoryHandler> logger)
        {
            _logger = logger;
            _newsContext = newsContext;
        }

        #region CRUD

        /// <summary>
        /// Tạo mới danh mục
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Thông tin danh mục</returns>
        public async Task<Response<CategoryResponse>> CreateCategoryAsync(CreatCategoryRequest request)
        {
            if (string.IsNullOrEmpty(request.Name))
            {
                _logger.LogError("Name category is null");
                return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { "Name category is null" });
            }
            var category = new Category
            {
                Id = Guid.NewGuid(),
                Name = request.Name
            };

            if (request.ParentId.HasValue)
            {
                var cate = await _newsContext.Categories.FindAsync(request.ParentId);
                if (cate == null)
                {
                    _logger.LogError("Not find parent category");
                    return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { "Not find parent category" });
                }
                category.ParentId = request.ParentId;
            }
            else
                category.ParentId = null;

            await _newsContext.Categories.AddAsync(category);

            var result = await _newsContext.SaveChangesAsync();

            var dataResponse = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId
            };

            if (result > 0) 
            {
                _logger.LogInformation("Create category success");
                return new Response<CategoryResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogError("Create category failed");
            return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> {"Error when save"});
        }

        /// <summary>
        /// Xóa danh mục theo id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>trả về status sucess và data = null</returns>
        public async Task<Response<CategoryResponse>> DeleteCategoryByIdAsync(Guid categoryId)
        {
            var category = await _newsContext.Categories.FindAsync(categoryId);

            if (category == null)
            {
                _logger.LogError("Not find category with id");
                return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { "Not find category with id" });
            }

            var categoryNews = _newsContext.CategoryNews.Where(x => x.CategoryId.Equals(categoryId));

            if (categoryNews != null || categoryNews.Any())
                _newsContext.CategoryNews.RemoveRange(categoryNews);

            _newsContext.Categories.Remove(category);

            var result = await _newsContext.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Delete category success");
                return new Response<CategoryResponse>(Constant.STATUS_SUCESS);
            }

            _logger.LogError("Delete category error");
            return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { "Error Save" });
        }

        /// <summary>
        /// Xem danh sách danh mục theo filter: tên,..
        /// Phân trang
        /// </summary>
        /// <param name="filter"></param>
        /// <returns>Danh sách danh mục theo filter</returns>
        public async Task<PaginatedList<CategoryResponse>> GetCategoryByFilterAsync(CategoryQueryFilter filter)
        {
            var dataResponse = new List<CategoryResponse>();

            foreach (var item in _newsContext.Categories)
            {
                dataResponse.Add(new CategoryResponse
                {
                    Id  = item.Id,
                    Name = item.Name,
                    ParentId = item.ParentId,
                    SubCategories = await GetSubCategoryAsync(item.Id)
                });
            }

            if (!string.IsNullOrEmpty(filter.Name))
                dataResponse = dataResponse.Where(x => x.Name.Contains(filter.Name)).ToList();

            if (filter.ParentId.HasValue)
                dataResponse = dataResponse.Where(x => x.ParentId == filter.ParentId).ToList();

            if (!filter.PageSize.HasValue && !filter.PageNumber.HasValue)
                return await PaginatedList<CategoryResponse>.CreateAsync(dataResponse);

            return await PaginatedList<CategoryResponse>.CreateAsync(dataResponse, filter.PageNumber.Value, filter.PageSize.Value);
        }

        /// <summary>
        /// Xem chi tiết danh mục theo id
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns></returns>
        public async Task<Response<CategoryResponse>> GetCategoryByIdAsync(Guid categoryId)
        {
            var category = await _newsContext.Categories.FindAsync(categoryId);

            if (category == null)
            {
                _logger.LogError("Not find category with id");
                return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { "Not find category with id" });
            }
            var dataResponse = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                SubCategories = await GetSubCategoryAsync(category.Id)
            };

            return new Response<CategoryResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }

        /// <summary>
        /// Cập nhật danh mục, thêm xóa tin tức khỏi danh mục
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        public async Task<Response<CategoryResponse>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryReqest request)
        {
            var category = await _newsContext.Categories.FindAsync(categoryId);

            if (category == null)
            {
                _logger.LogError("Not find category with id");
                return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { "Not find category with id" });
            }

            if (!string.IsNullOrEmpty(request.Name))
                category.Name = request.Name;

            if (request.ParentId.HasValue)
                category.ParentId = request.ParentId;

            //Thêm danh sách tin tức vào danh mục
            foreach (var newsId in request.AddNewsIds ?? new List<Guid> { })
            {
                var news = await _newsContext.News.FindAsync(newsId);
                if (news != null)
                {
                    var categoryNews = _newsContext.CategoryNews.Where(x => x.CategoryId.Equals(category.Id) && x.NewsId.Equals(news.Id));

                    if (categoryNews != null && categoryNews.Any())
                    {
                        _logger.LogError($"News already in '{category.Name}'");
                        return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { $"News already in '{category.Name}'" });
                    }

                    await _newsContext.CategoryNews.AddAsync(new CategoryNews
                    {
                        Id = Guid.NewGuid(),
                        CategoryId = category.Id,
                        NewsId = news.Id
                    });
                }
            }

            //Xóa danh sách tin tức khỏi danh mục
            foreach (var newsId in request.RemoveNewsIds ?? new List<Guid> { })
            {
                var news = await _newsContext.News.FindAsync(newsId);
                if (news != null)
                {
                    var categoryNews = _newsContext.CategoryNews.Where(x => x.NewsId.Equals(newsId));
                    if (categoryNews != null && categoryNews.Any())
                        _newsContext.CategoryNews.RemoveRange(categoryNews);
                }
            }

            var dataResponse = new CategoryResponse
            {
                Id = category.Id,
                Name = category.Name,
                ParentId = category.ParentId,
                SubCategories = await GetSubCategoryAsync(category.Id)
            };

            var result = await _newsContext.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Update category success");
                return new Response<CategoryResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
            }

            _logger.LogError("Update category error");
            return new Response<CategoryResponse>(Constant.STATUS_ERROR, new List<string> { "Error when save" });
        }

        #endregion

        /// <summary>
        /// Xem danh sách danh mục của tin tức
        /// </summary>
        /// <param name="newsId"></param>
        /// <returns></returns>
        public async Task<Response<GetCategoryByNewsIdResponse>> GetCategoryByNewsIdAsync(Guid newsId)
        {
            var news = await _newsContext.News.FindAsync(newsId);

            if (news == null)
            {
                _logger.LogError("Not find news with id");
                return new Response<GetCategoryByNewsIdResponse>(Constant.STATUS_ERROR, new List<string> { "No find news with id" });
            }

            var dataResponse = new GetCategoryByNewsIdResponse();
            //Ds danh mục thuộc tin tức
            var categories = new List<CategoryResponse>();
            //Ds danh mục k thuộc tin tức
            var noCategories = new List<CategoryResponse>();

            //Thêm danh mục vào 2 ds trên
            foreach (var item in _newsContext.Categories)
            {
                var list = IsNewsInCategory(news, item) ? categories : noCategories;
                list.Add(new CategoryResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    ParentId = item.ParentId,
                    SubCategories = await GetSubCategoryAsync(item.Id)
                });
            }

            dataResponse.CategoryHaveNews = categories;
            dataResponse.CategoryNoHaveNews = noCategories;

            return new Response<GetCategoryByNewsIdResponse>(Constant.STATUS_SUCESS, null, dataResponse, 1);
        }
        
        /// <summary>
        /// Lấy danh sách danh mục con của danh mục cha
        /// </summary>
        /// <param name="categoryId"></param>
        /// <returns>Danh sách danh mục con</returns>
        private async Task<List<CategoryResponse>> GetSubCategoryAsync(Guid categoryId)
        {
            var category = await _newsContext.Categories.Where(x => x.ParentId == categoryId).ToListAsync();

            if (category == null || !category.Any())
                return null;

            var categories = new List<CategoryResponse>();

            foreach (var item in category)
            {
                categories.Add(new CategoryResponse
                {
                    Id = item.Id,
                    Name = item.Name,
                    ParentId = item.ParentId,
                    SubCategories = await GetSubCategoryAsync(item.Id)
                });
            }

            return categories;
        }

        /// <summary>
        /// Kiểm tra tin tức có trong danh mục không
        /// </summary>
        /// <param name="news"></param>
        /// <param name="category"></param>
        /// <returns></returns>
        private bool IsNewsInCategory(News news, Category category)
        {
            var categoryNews = _newsContext.CategoryNews.Where(x => x.NewsId.Equals(news.Id) && x.CategoryId.Equals(category.Id));
            if (categoryNews != null && categoryNews.Any())
                return true;

            return false;
        }
    }
}
