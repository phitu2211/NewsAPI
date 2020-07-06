using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace NewsAPI.Data.Context
{
    public class Log
    {
        [Key]
        public Guid Id { get; set; }

        public string Level { get; set; }

        public string Message { get; set; }
    }
}
