using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CrumbContentfulFactory : IContentfulFactory<Entry<ContentfulCrumb>, Crumb>
    {
        public Crumb ToModel(Entry<ContentfulCrumb> entry)
        {
            var title = !string.IsNullOrEmpty(entry.Fields.Title) ? entry.Fields.Title : entry.Fields.Name;

            return new Crumb(title, entry.Fields.Slug, entry.SystemProperties.ContentType.SystemProperties.Id);
        }
    }
}