using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NewsAPI.Business.V1;
using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using Newtonsoft.Json;

namespace NewsAPI.Controllers.V1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryHandler _categoryService;

        public CategoryController(ICategoryHandler categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet(Constant.ApiRoutes.Category.GetCategoryByFilter)]
        public async Task<PaginatedList<CategoryResponse>> GetCategoryByFilter(string filter = "{}")
        {
            var filterConvert = JsonConvert.DeserializeObject<CategoryQueryFilter>(filter);

            return await _categoryService.GetCategoryByFilterAsync(filterConvert);
        }

        [HttpGet(Constant.ApiRoutes.Category.GetCategoryById)]
        public async Task<Response<CategoryResponse>> GetCategoryById([FromRoute] Guid categoryId)
        {
            return await _categoryService.GetCategoryByIdAsync(categoryId);
        }

        [HttpGet(Constant.ApiRoutes.Category.GetCategoryByNewsId)]
        public async Task<Response<GetCategoryByNewsIdResponse>> GetCategoryByNewsId([FromRoute] Guid newsId)
        {
            return await _categoryService.GetCategoryByNewsIdAsync(newsId);
        }

        [HttpPost(Constant.ApiRoutes.Category.CreateCategory)]
        public async Task<Response<CategoryResponse>> CreateCategory([FromBody] CreatCategoryRequest request)
        {
            return await _categoryService.CreateCategoryAsync(request);
        }

        [HttpPut(Constant.ApiRoutes.Category.UpdateCategory)]
        public async Task<Response<CategoryResponse>> UpdateCategory([FromRoute] Guid categoryId, [FromBody] UpdateCategoryReqest request)
        {
            return await _categoryService.UpdateCategoryAsync(categoryId, request);
        }

        [HttpDelete(Constant.ApiRoutes.Category.DeleteCategory)]
        public async Task<Response<CategoryResponse>> DeleteCategoryById([FromRoute] Guid categoryId)
        {
            return await _categoryService.DeleteCategoryByIdAsync(categoryId);
        }
    }
}