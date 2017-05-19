using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulEventBannerBuilder
    {
        private string _title = "title";
        private string _teaser = "teaser";
        private string _icon = "icon";
        private string _link = "link";

        public  ContentfulEventBanner Build()
        {
            return new ContentfulEventBanner
            {
                Title = _title,
                Teaser = _teaser,
                Icon = _icon,
                Link = _link
            };
        }

    }
}
