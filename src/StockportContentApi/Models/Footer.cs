namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Footer
{
    public string Title { get; }
    public string Slug { get; }
    public IEnumerable<SubItem> Links { get; }
    public IEnumerable<SocialMediaLink> SocialMediaLinks { get; }
    public string FooterContent1 { get; }
    public string FooterContent2 { get; }
    public string FooterContent3 { get; }

    public Footer(string title, 
        string slug, 
        IEnumerable<SubItem> subItems, 
        IEnumerable<SocialMediaLink> socialMediaLinks, 
        string footerContent1, 
        string footerContent2, 
        string footerContent3)
    {
        Title = title;
        Slug = slug;
        Links = subItems;
        SocialMediaLinks = socialMediaLinks;
        FooterContent1 = footerContent1;
        FooterContent2 = footerContent2;
        FooterContent3 = footerContent3;
    }
}