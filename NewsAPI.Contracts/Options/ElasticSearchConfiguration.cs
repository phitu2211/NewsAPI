using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NewsAPI.Contracts.Options
{
    public class ElasticSearchConfiguration
    {
        public bool IsUse { get; set; }
        public string Uri { get; set; }
        public string DefaultIndex { get; set; }
    }
}
