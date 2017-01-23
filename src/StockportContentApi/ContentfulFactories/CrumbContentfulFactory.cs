using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CrumbContentfulFactory : IContentfulFactory<Entry<ContentfulCrumb>, Crumb>
    {
        public Crumb ToModel(Entry<ContentfulCrumb> entry)
        {
            return new Crumb(entry.Fields.Title, entry.Fields.Slug, entry.SystemProperties.Id);
        }
    }
}