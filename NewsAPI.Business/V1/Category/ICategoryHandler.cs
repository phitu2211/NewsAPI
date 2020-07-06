using NewsAPI.Contracts.V1.Helper;
using NewsAPI.Contracts.V1.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace NewsAPI.Business.V1
{
    public interface ICategoryHandler
    {
        Task<PaginatedList<CategoryResponse>> GetCategoryByFilterAsync(CategoryQueryFilter filter);
        Task<Response<CategoryResponse>> GetCategoryByIdAsync(Guid categoryId);
        Task<Response<CategoryResponse>> CreateCategoryAsync(CreatCategoryRequest request);
        Task<Response<CategoryResponse>> UpdateCategoryAsync(Guid categoryId, UpdateCategoryReqest request);
        Task<Response<CategoryResponse>> DeleteCategoryByIdAsync(Guid categoryId);
        Task<Response<GetCategoryByNewsIdResponse>> GetCategoryByNewsIdAsync(Guid newsId);
    }
}
