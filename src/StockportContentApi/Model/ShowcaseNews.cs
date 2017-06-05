using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class ShowcaseNews
    {
        public string Type { get; set; } = string.Empty;

        public News News { get; set; } = null;
    }
}
