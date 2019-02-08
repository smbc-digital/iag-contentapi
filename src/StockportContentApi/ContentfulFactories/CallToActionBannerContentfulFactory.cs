using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CallToActionBannerContentfulFactory : IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>
    {
        public CallToActionBanner ToModel(ContentfulCallToActionBanner entry)
        {
            return new CallToActionBanner()
            {
                AltText = entry.AltText,
                ButtonText = entry.ButtonText,
                Image = entry.Image.File.Url,
                Link = entry.Link,
                Title = entry.Title
            };
        }
    }
}
