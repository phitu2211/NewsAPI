using Nest;
using System;
using System.Collections.Generic;
using System.Text;

namespace NewsAPI.Contracts.V1.Model
{
    [ElasticsearchType(RelationName = "news-logging")]
    public class LogModel
    {
        [Text(Name = "_id")]
        public Guid Id { get; set; }
        [Text(Name = "level")]
        public string Level { get; set; }
        [Text(Name = "message")]
        public string Message { get; set; }
    }

    public class LogQueryFilter : LogModel
    {
        public int? PageSize { get; set; }
        public int? PageNumber { get; set; }
        public LogQueryFilter()
        {
            PageNumber = 1;
            PageSize = 10;
        }
        public LogQueryFilter(int pageSize, int pageNumber)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
    }
}
