using System;
using System.Collections.Generic;
using System.Text;

namespace NewsAPI.Contracts.V1.Model
{
    public class CategoryModel
    {
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
    }

    public class CategoryResponse : CategoryModel
    {
        public Guid Id { get; set; }
        public List<CategoryResponse> SubCategories { get; set; }
    }

    public class CategoryQueryFilter : CategoryModel
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public CategoryQueryFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public CategoryQueryFilter(int pageSize, int pageNumber)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class CreatCategoryRequest : CategoryModel
    {

    }

    public class UpdateCategoryReqest : CategoryModel
    {
        public List<Guid> AddNewsIds { get; set; }
        public List<Guid> RemoveNewsIds { get; set; }
    }

    public class GetCategoryByNewsIdResponse
    {
        public List<CategoryResponse> CategoryHaveNews { get; set; }
        public List<CategoryResponse> CategoryNoHaveNews { get; set; }
    }
}
