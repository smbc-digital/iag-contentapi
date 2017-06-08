using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CrumbContentfulFactory : IContentfulFactory<ContentfulCrumb, Crumb>
    {
        public Crumb ToModel(ContentfulCrumb entry)
        {
            var title = !string.IsNullOrEmpty(entry.Title) ? entry.Title : entry.Name;

            return new Crumb(title, entry.Slug, entry.Sys.ContentType.SystemProperties.Id);
        }
    }
}