using System;
using System.Collections.Generic;
using System.Text;

namespace NewsAPI.Contracts.V1.Model
{
    public class NewsModel
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public string UrlImage { get; set; }
    }

    public class NewsResponse : NewsModel
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public List<CategoryResponse> Categories { get; set; }
    }

    public class CreateNewsRequest : NewsModel
    {
        public List<Guid> CategoryIds { get; set; }
    }

    public class UpdateNewsRequest : NewsModel
    {
        public List<Guid> AddCategoryIds { get; set; }
        public List<Guid> RemoveCategoryIds { get; set; }
    }

    public class NewsQueryFilter : NewsModel
    {
        //public string CategoryName { get; set; }
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public DateTime CreateFromDate { get; set; }
        public DateTime CreateToDate { get; set; }
        public DateTime UpdateFromDate { get; set; }
        public DateTime UpdateToDate { get; set; }
        public List<Guid> CategoryIds { get; set; }
        public NewsQueryFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public NewsQueryFilter(int pageSize, int pageNumber)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }

    public class GetNewsByCategoryIdResponse
    {
        public List<NewsResponse> NewsBelongCategories { get; set; }
        public List<NewsResponse> NewsNoBelongCategories { get; set; }
    }
}
