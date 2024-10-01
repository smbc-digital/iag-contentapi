namespace StockportContentApiTests.Unit.Builders;

public class ContentfulFooterBuilder
{
    private string _title { get; set; } = "Footer";
    private string _slug { get; set; } = "a-slug";
    private string _copyrightSection { get; set; } = "© 2016 A Council Name";
    private readonly List<ContentfulReference> _links = new()
    {
      new ContentfulReferenceBuilder().Build()
    };
    private readonly List<ContentfulSocialMediaLink> _socialMediaLinks = new();

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
