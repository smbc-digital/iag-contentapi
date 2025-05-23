namespace StockportContentApiTests.Unit.Builders;

public class ContentfulFooterBuilder
{
    private string _title { get; set; } = "Footer";
    private string _slug { get; set; } = "a-slug";
    private readonly List<ContentfulReference> _links = new()
    {
      new ContentfulReferenceBuilder().Build()
    };
    private readonly List<ContentfulSocialMediaLink> _socialMediaLinks = new();

    public ContentfulFooter Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Links = _links,
            SocialMediaLinks = _socialMediaLinks
        };
}