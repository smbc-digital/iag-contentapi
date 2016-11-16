using System.Collections.Generic;
using StockportContentApi.Model;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class SocialMediaLinkListFactory : IBuildContentTypesFromReferences<SocialMediaLink>
    {
        private readonly IFactory<SocialMediaLink> _socialMediaLinkFactory;

        public SocialMediaLinkListFactory(IFactory<SocialMediaLink> socialMediaLinkFactory)
        {
            _socialMediaLinkFactory = socialMediaLinkFactory;
        }

        public IEnumerable<SocialMediaLink> BuildFromReferences(IEnumerable<dynamic> references, IContentfulIncludes contentfulResponse)
        {
            if (references == null) return new List<SocialMediaLink>();
            IEnumerable<dynamic> entries = contentfulResponse.GetEntriesAndItemsFor(references);

            if (entries == null) return new List<SocialMediaLink>();
            return entries
                .Select(item => _socialMediaLinkFactory.Build(item, contentfulResponse))
                .Cast<SocialMediaLink>()
                .ToList();
        }
    }
}