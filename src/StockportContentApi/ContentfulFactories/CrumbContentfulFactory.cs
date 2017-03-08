using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class CrumbContentfulFactory : IContentfulFactory<Entry<ContentfulCrumb>, Crumb>
    {
        private readonly IContentfulFactory<Entry<ContentfulSubItem>, SubItem> _subItemFactory;

        public CrumbContentfulFactory(IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subItemFactory)
        {
            _subItemFactory = subItemFactory;
        }

        public Crumb ToModel(Entry<ContentfulCrumb> entry)
        {
            var title = !string.IsNullOrEmpty(entry.Fields.Title) ? entry.Fields.Title : entry.Fields.Name;

            return new Crumb(title, entry.Fields.Slug, entry.SystemProperties.ContentType.SystemProperties.Id);
        }
    }
}