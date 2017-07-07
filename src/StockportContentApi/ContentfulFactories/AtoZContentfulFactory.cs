using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using System.Collections.Generic;

namespace StockportContentApi.ContentfulFactories
{
    public class AtoZContentfulFactory : IContentfulFactory<ContentfulAtoZ, AtoZ>
    {       
        public AtoZ ToModel(ContentfulAtoZ entry)
        {
            var type = entry.Sys.ContentType.SystemProperties.Id ?? string.Empty;
            return new AtoZ(entry.Title, entry.Slug, entry.Teaser, type, entry.AlternativeTitles);
        }
    }
}