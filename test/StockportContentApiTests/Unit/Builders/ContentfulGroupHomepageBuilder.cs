namespace StockportContentApiTests.Unit.Builders;

public class ContentfulGroupHomepageBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _metaDescription = "metaDescription";
    private readonly Asset _backgroundImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
    };
    private ContentfulEventBanner _eventBanner =
        new ContentfulEventBannerBuilder().Build();
    private readonly string _featuredGroupsHeading = "heading";
    private readonly List<ContentfulGroup> _featuredGroups = new()
    {
        new ContentfulGroupBuilder().Build()
    };

    public ContentfulGroupHomepage Build()
    {
        return new ContentfulGroupHomepage
        {
            Slug = _slug,
            MetaDescription = _metaDescription,
            Title = _title,
            BackgroundImage = _backgroundImage,
            EventBanner = _eventBanner,
            FeaturedGroupsHeading = _featuredGroupsHeading,
            FeaturedGroups = _featuredGroups,
            Alerts = new List<ContentfulAlert>
            {
                new ContentfulAlertBuilder().Build()
            },
            BodyHeading = "bodyheading",
            Body = "body",
            SecondaryBodyHeading = "secondaryBodyHeading",
            SecondaryBody = "secondaryBody"
        };
    }

    public ContentfulGroupHomepageBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }
    public ContentfulGroupHomepageBuilder MetaDescription(string metaDescription)
    {
        _metaDescription = metaDescription;
        return this;
    }
    public ContentfulGroupHomepageBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulGroupHomepageBuilder EventBanner(ContentfulEventBanner banner)
    {
        _eventBanner = banner;
        return this;
    }

}
