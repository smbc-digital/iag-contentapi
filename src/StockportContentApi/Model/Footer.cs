using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Footer
    {
        public string Title { get; }
        public string Slug { get; }
        public string CopyrightSection { get; }
        public IEnumerable<SubItem> Links { get; }
        public IEnumerable<SocialMediaLink> SocialMediaLinks { get; }

        public Footer(string title, string slug, string copyrightSection, IEnumerable<SubItem> subItems, IEnumerable<SocialMediaLink> socialMediaLinks)
        {
            Title = title;
            Slug = slug;
            CopyrightSection = copyrightSection;
            Links = subItems;
            SocialMediaLinks = socialMediaLinks;
        }
    }

}

