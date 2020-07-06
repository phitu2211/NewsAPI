using System;
using System.Collections.Generic;
using System.Text;

namespace NewsAPI.Data.Context
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid? ParentId { get; set; }
    }
}
