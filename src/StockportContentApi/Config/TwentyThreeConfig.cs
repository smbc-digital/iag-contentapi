using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Config
{
    public class TwentyThreeConfig
    {
        public TwentyThreeConfig(string baseUrl)
        {
            Ensure.ArgumentNotNullOrEmpty(baseUrl, "TWENTY_THREE_BASEURL");

            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }
    }
}
