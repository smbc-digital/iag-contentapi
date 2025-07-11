﻿namespace StockportContentApiTests.Unit.Builders;

public class ContentfulTopicBuilder
{
    private string _title = "name";
    private string _slug = "slug";
    private string _icon = "icon";
    private string _summary = "summary";
    private string _teaser = "teaser";
    private string _metaDescription = "metaDescription";
    private DateTime _sunriseDate = DateTime.MinValue;
    private DateTime _sunsetDate = DateTime.MaxValue;
    private Asset _backgroundImage = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private Asset _image = new ContentfulAssetBuilder().Url("background-image-url.jpg").Build();
    private List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() };
    private List<ContentfulAlert> _alerts = new(){ new ContentfulAlertBuilder().Build() };
    private List<ContentfulReference> _subItems = new() { new ContentfulReferenceBuilder().Slug("sub-slug").Build() };
    private List<ContentfulReference> _secondaryItems = new() { new ContentfulReferenceBuilder().Slug("secondary-slug").Build() };
    private ContentfulCallToActionBanner _callToActionBanner = new ContentfulCallToActionBannerBuilder().Build();
    private ContentfulEventBanner _eventBanner = new ContentfulEventBannerBuilder().Build();
    private string _systemId = "id";
    private string _contentTypeSystemId = "id";

    public ContentfulTopic Build() => new()
        {
            Slug = _slug,
            Title = _title,
            Teaser = _teaser,
            MetaDescription = _metaDescription,
            Summary = _summary,
            Icon = _icon,
            BackgroundImage = _backgroundImage,
            Image = _image,
            SubItems = _subItems,
            SecondaryItems = _secondaryItems,
            CallToAction = _callToActionBanner,
            Breadcrumbs = _breadcrumbs,
            Alerts = _alerts,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            EventBanner = _eventBanner,
            DisplayContactUs = false,
            Sys = new SystemProperties()
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                Id = _systemId
            }
        };

    public ContentfulTopicBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulTopicBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulTopicBuilder Teaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }
    public ContentfulTopicBuilder MetaDescription(string metaDescription)
    {
        _metaDescription = metaDescription;
        return this;
    }

    public ContentfulTopicBuilder Summary(string summary)
    {
        _summary = summary;
        return this;
    }

    public ContentfulTopicBuilder Icon(string icon)
    {
        _icon = icon;
        return this;
    }

    public ContentfulTopicBuilder BackgroundImage(Asset image)
    {
        _backgroundImage = image;
        return this;
    }

    public ContentfulTopicBuilder Image(Asset image)
    {
        _image = image;
        return this;
    }

    public ContentfulTopicBuilder SubItems(List<ContentfulReference> subItems)
    {
        _subItems = subItems;
        return this;
    }

    public ContentfulTopicBuilder SecondaryItems(List<ContentfulReference> items)
    {
        _secondaryItems = items;
        return this;
    }

    public ContentfulTopicBuilder CallToActionBanner(ContentfulCallToActionBanner callToAction)
    {
        _callToActionBanner = callToAction;
        return this;
    }

    public ContentfulTopicBuilder Breadcrumbs(List<ContentfulReference> crumb)
    {
        _breadcrumbs = crumb;
        return this;
    }

    public ContentfulTopicBuilder Alerts(List<ContentfulAlert> alerts)
    {
        _alerts = alerts;
        return this;
    }

    public ContentfulTopicBuilder SunriseDate(DateTime date)
    {
        _sunriseDate = date;
        return this;
    }

    public ContentfulTopicBuilder SunsetDate(DateTime date)
    {
        _sunsetDate = date;
        return this;
    }

    public ContentfulTopicBuilder EventBanner(ContentfulEventBanner banner)
    {
        _eventBanner = banner;
        return this;
    }

    public ContentfulTopicBuilder SystemId(string id)
    {
        _systemId = id;
        return this;
    }

    public ContentfulTopicBuilder SystemContentTypeId(string id)
    {
        _contentTypeSystemId = id;
        return this;
    }
}