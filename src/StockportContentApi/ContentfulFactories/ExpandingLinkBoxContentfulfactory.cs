using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ExpandingLinkBoxContentfulfactory : IContentfulFactory<Entry<ContentfulExpandingLinkBox>, ExpandingLinkBox>
    {
        private IContentfulFactory<Entry<ContentfulSubItem>, SubItem> _subitemFactory;

        public ExpandingLinkBoxContentfulfactory(IContentfulFactory<Entry<ContentfulSubItem>, SubItem> subitemFactory)
        {
            _subitemFactory = subitemFactory;
        }

        public ExpandingLinkBox ToModel(Entry<ContentfulExpandingLinkBox> entry)
        {
            return new ExpandingLinkBox(entry.Fields.Title,
                entry.Fields.Links.Select(e => _subitemFactory.ToModel(e)).ToList());
        }
    }
}
