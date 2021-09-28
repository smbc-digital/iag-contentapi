using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CrumbContentfulFactory : IContentfulFactory<ContentfulReference, Crumb>
    {
        public Crumb ToModel(ContentfulReference entry)
        {
            var title = !string.IsNullOrEmpty(entry.Title) ? entry.Title : entry.Name;

            return new Crumb(title, entry.Slug, entry.Sys.ContentType.SystemProperties.Id);
        }
    }
}