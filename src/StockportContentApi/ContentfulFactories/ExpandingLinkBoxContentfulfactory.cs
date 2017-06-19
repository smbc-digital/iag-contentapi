using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ExpandingLinkBoxContentfulfactory : IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>
    {
        private IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;

        public ExpandingLinkBoxContentfulfactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory)
        {
            _subitemFactory = subitemFactory;
        }

        public ExpandingLinkBox ToModel(ContentfulExpandingLinkBox entry)
        {
            return new ExpandingLinkBox(entry.Title,
                entry.Links.Select(e => _subitemFactory.ToModel(e)).ToList());
        }
    }
}
