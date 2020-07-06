using System;
using System.Collections.Generic;
using System.Text;

namespace NewsAPI.Data.Context
{
    public class CategoryNews
    {
        public Guid Id { get; set; }
        public Guid CategoryId { get; set; }
        public Guid NewsId { get; set; }
    }
}
