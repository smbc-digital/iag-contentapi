namespace StockportContentApiTests.Unit.Builders;

public class ContentfulFooterBuilder
{
    private readonly List<ContentfulReference> _links = new()
    {
		new ContentfulReferenceBuilder().Build()
    };

  private readonly List<ContentfulSocialMediaLink> _socialMediaLinks = new();

    public ContentfulFooter Build()
        => new()
        {
            Title = "Footer",
            Slug = "a-slug",
            Links = _links,
            SocialMediaLinks = _socialMediaLinks
        };
}