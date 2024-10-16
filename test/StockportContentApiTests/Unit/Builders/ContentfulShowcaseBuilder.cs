namespace StockportContentApiTests.Builders;

public class ContentfulShowcaseBuilder
{
    private string _title { get; set; } = "title";
    private string _slug { get; set; } = "slug";
    private string _teaser { get; set; } = "teaser";
    private string _metaDescription { get; set; } = "metaDescription";
    private string _subheading { get; set; } = "subheading";
    private Asset _heroImage { get; set; } = new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } };
    private List<ContentfulReference> _secondaryItems { get; set; } = new List<ContentfulReference>
    {
        new ContentfulReferenceBuilder().Build()
    };
    private string _eventSubheading { get; set; } = "event-subheading";
    private string _eventCategory { get; set; } = "event-category";
    private string _newsSubheading { get; set; } = "news subheading";
    private string _newsCategoryTag { get; set; } = "news-category-tag";
    private string _bodySubheading { get; set; } = "body subheading";
    private string _body { get; set; } = "body";
    private string _emailAlertsTopicId { get; set; } = "alertId";
    private string _emailAlertsText { get; set; } = "alertText";
    private string _typeformUrl { get; set; } = "typeformUrl";
    private List<ContentfulSocialMediaLink> _socialMediaLinks = new();
    private List<ContentfulReference> _breadcrumbs = new()
    {
      new ContentfulReferenceBuilder().Build()
    };

    private readonly List<ContentfulAlert> _alerts = new()
    {
        new ContentfulAlertBuilder().Build()
    };

    public ContentfulShowcase Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            MetaDescription = _metaDescription,
            Subheading = _subheading,
            HeroImage = _heroImage,
            EventSubheading = _eventSubheading,
            EventCategory = _eventCategory,
            NewsSubheading = _newsSubheading,
            NewsCategoryTag = _newsCategoryTag,
            SecondaryItems = _secondaryItems,
            Breadcrumbs = _breadcrumbs,
            SocialMediaLinks = _socialMediaLinks,
            BodySubheading = _bodySubheading,
            Body = _body,
            EmailAlertsText = _emailAlertsText,
            EmailAlertsTopicId = _emailAlertsTopicId,
            Alerts = _alerts,
            TypeformUrl = _typeformUrl
        };

    public ContentfulShowcaseBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulShowcaseBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulShowcaseBuilder HeroImage(Asset heroImage)
    {
        _heroImage = heroImage;
        return this;
    }

    public ContentfulShowcaseBuilder Teaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }
    public ContentfulShowcaseBuilder MetaDescription(string metaDescription)
    {
        _metaDescription = metaDescription;
        return this;
    }

    public ContentfulShowcaseBuilder Subheading(string subheading)
    {
        _subheading = subheading;
        return this;
    }

    public ContentfulShowcaseBuilder EventSubheading(string subheading)
    {
        _eventSubheading = subheading;
        return this;
    }

    public ContentfulShowcaseBuilder EventCategory(string cat)
    {
        _eventCategory = cat;
        return this;
    }

    public ContentfulShowcaseBuilder NewsSubheading(string newSubheading)
    {
        _newsSubheading = newSubheading;
        return this;
    }

    public ContentfulShowcaseBuilder NewsCategoryTag(string tag)
    {
        _newsCategoryTag = tag;
        return this;
    }

    public ContentfulShowcaseBuilder BodySubheading(string subheading)
    {
        _bodySubheading = subheading;
        return this;
    }

    public ContentfulShowcaseBuilder Body(string body)
    {
        _body = body;
        return this;
    }

    public ContentfulShowcaseBuilder SecondaryItems(List<ContentfulReference> secondaryItems)
    {
        _secondaryItems = secondaryItems;
        return this;
    }

    public ContentfulShowcaseBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
    {
        _breadcrumbs = breadcrumbs;
        return this;
    }

    public ContentfulShowcaseBuilder SocialMediaLinks(List<ContentfulSocialMediaLink> socialMediaLinks)
    {
        _socialMediaLinks = socialMediaLinks;
        return this;
    }

    public ContentfulShowcaseBuilder TypeformUrl(string typeformUrl)
    {
        _typeformUrl = typeformUrl;
        return this;
    }
}