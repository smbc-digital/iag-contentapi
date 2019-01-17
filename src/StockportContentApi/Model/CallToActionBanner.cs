using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class CallToActionBanner
    {
        public string Title { get; set; } = string.Empty;
        public string Image { get; set; } = null;
        public string Link { get; set; } = string.Empty;
        public string ButtonText { get; set; } = string.Empty;
        public string AltText { get; set; } = string.Empty;
    }
}
