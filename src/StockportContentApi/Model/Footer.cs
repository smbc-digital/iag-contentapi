using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.Model
{
    public class Footer
    {
        public string Title { get; }
        public string Slug { get; }
        public string Copyright { get; }
        public IEnumerable<SubItem> Links { get; }
        public IEnumerable<SocialMediaLink> SocialMediaLinks { get; }

        public Footer(string title, string slug, string copyright, IEnumerable<SubItem> subItems, IEnumerable<SocialMediaLink> socialMediaLinks)
        {
            Title = title;
            Slug = slug;
            Copyright = copyright;
            Links = subItems;
            SocialMediaLinks = socialMediaLinks;
        }
    }

}

