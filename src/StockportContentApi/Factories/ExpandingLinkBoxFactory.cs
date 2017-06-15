using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class ExpandingLinkBoxFactory : IBuildContentTypesFromReferences<ExpandingLinkBox>
    {
        private readonly IFactory<SubItem> _subitemFactory;

        public ExpandingLinkBoxFactory(IFactory<SubItem> subitemFactory)
        {
            _subitemFactory = subitemFactory;          
        }

        public IEnumerable<ExpandingLinkBox> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<ExpandingLinkBox>();
            var expandingListBoxEntries = contentfulResponse.GetEntriesFor(references);

            if (expandingListBoxEntries == null) return new List<ExpandingLinkBox>();
            return expandingListBoxEntries
               .Select(item => BuildExpandingLinkBox(item, contentfulResponse))
               .Cast<ExpandingLinkBox>()
               .ToList();
        }

        public ExpandingLinkBox BuildExpandingLinkBox(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            string title = entry.fields.title ?? "";
            var links = _subitemFactory.Build(entry, contentfulResponse);

            return new ExpandingLinkBox(title, links);
        }
    }
}

