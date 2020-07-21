using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsAPI.Data.Context
{
    public class Menu
    {
        [Key]
        public Guid Id { get; set; }
        public string MenuName { get; set; }
        public string Href { get; set; }
        public string Icon { get; set; }
        public Guid? ParentId { get; set; }
    }
}
