using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Contracts.V1.Helper
{
    public class PaginatedList<T>
    {
        public int? PageIndex { get; set; }
        public int? TotalPage { get; set; }
        public int? TotalData { get; set; }
        public IEnumerable<T> Data { get; set; }

        public PaginatedList(IEnumerable<T> items, int count, int? pageIndex, int? pageSize)
        {
            if(null == pageIndex || null == pageSize)
            {
                PageIndex = null;
                TotalPage = null;
                TotalData = count;
                Data = items;
            }
            else
            {
                TotalData = count;
                PageIndex = pageIndex;
                TotalPage = (int)Math.Ceiling(count / (double)pageSize);
                Data = items;
            }
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get { return (PageIndex < TotalPage); }
        }

        public static async Task<PaginatedList<T>> CreateAsync(IEnumerable<T> source, int pageIndex = 1, int pageSize = -1)
        {
            var count = source.Count();
            if(pageSize < 0)
                return new PaginatedList<T>(source, count, null, null);
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
