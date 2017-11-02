using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class SmartResultContentfulFactory : IContentfulFactory<ContentfulSmartResult, SmartResult>
    {
        public SmartResult ToModel(ContentfulSmartResult entry)
        {
            var buttonText = string.IsNullOrEmpty(entry.ButtonText) ? "Go to homepage" : entry.ButtonText;
            var buttonLink = string.IsNullOrEmpty(entry.ButtonLink) ? "https://www.stockport.gov.uk/" : entry.ButtonLink;

            return new SmartResult(entry.Title, entry.Slug, entry.Subheading, entry.Icon, entry.Body, buttonText, buttonLink);         
        }
    }
}
