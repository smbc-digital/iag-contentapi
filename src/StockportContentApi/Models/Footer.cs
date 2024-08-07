﻿namespace StockportContentApi.Model;

[ExcludeFromCodeCoverage]
public class Footer
{
    public string Title { get; }
    public string Slug { get; }
    public IEnumerable<SubItem> Links { get; }
    public IEnumerable<SocialMediaLink> SocialMediaLinks { get; }

    public Footer(string title, string slug, IEnumerable<SubItem> subItems, IEnumerable<SocialMediaLink> socialMediaLinks)
    {
        Title = title;
        Slug = slug;
        Links = subItems;
        SocialMediaLinks = socialMediaLinks;
    }
}