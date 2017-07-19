using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulFooterBuilder
    {
        private string _title { get; set; } = "Footer";
        private string _slug { get; set; } = "a-slug";
        private string _copyrightSection { get; set; } = "© 2016 A Council Name";
        private List<ContentfulReference> _links = new List<ContentfulReference>
        {
          new ContentfulReferenceBuilder().Build()
        };
        private List<ContentfulSocialMediaLink> _socialMediaLinks = new List<ContentfulSocialMediaLink>();

        public ContentfulFooter Build()
        {
            return new ContentfulFooter()
            {
                Title = _title,
                Slug = _slug,
                CopyrightSection = _copyrightSection,
                Links = _links,
                SocialMediaLinks = _socialMediaLinks
            };
        }      
    }
}
