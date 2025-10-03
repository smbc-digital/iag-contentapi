namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Footer(string title,
                    string slug,
                    IEnumerable<SubItem> subItems,
                    IEnumerable<SocialMediaLink> socialMediaLinks,
                    string footerContent1,
                    string footerContent2,
                    string footerContent3)
{
    public string Title { get; } = title;
    public string Slug { get; } = slug;
    public IEnumerable<SubItem> Links { get; } = subItems;
    public IEnumerable<SocialMediaLink> SocialMediaLinks { get; } = socialMediaLinks;
    public string FooterContent1 { get; } = footerContent1;
    public string FooterContent2 { get; } = footerContent2;
    public string FooterContent3 { get; } = footerContent3;
}