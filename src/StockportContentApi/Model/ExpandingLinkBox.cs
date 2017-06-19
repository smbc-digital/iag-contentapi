using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class ExpandingLinkBox
    {
        public string Title { get; set; }
        public List<SubItem> Links { get; set; }

        public ExpandingLinkBox(string title, List<SubItem> links)
        {
            Title = title;
            Links = links;
        }
    }
}
