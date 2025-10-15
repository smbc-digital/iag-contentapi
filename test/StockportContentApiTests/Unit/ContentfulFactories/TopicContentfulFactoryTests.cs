namespace StockportContentApiTests.Unit.ContentfulFactories;

public class TopicContentfulFactoryTests
{
    private readonly ContentfulTopic _contentfulTopic;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>> _carouselFactory = new();
    private readonly TopicContentfulFactory _topicFactory;
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _topicBrandingFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();

    public TopicContentfulFactoryTests()
    {
        _contentfulTopic = new ContentfulTopicBuilder().Build();
        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2017, 02, 02));

        _callToActionFactory
            .Setup(callToActionFactory => callToActionFactory.ToModel(It.IsAny<ContentfulCallToActionBanner>()))
            .Returns(new CallToActionBanner());

        _topicFactory = new TopicContentfulFactory(_subItemFactory.Object,
                                                _crumbFactory.Object,
                                                _alertFactory.Object,
                                                _eventBannerFactory.Object,
                                                _carouselFactory.Object,
                                                _timeProvider.Object,
                                                _callToActionFactory.Object,
                                                _topicBrandingFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopic()
    {
        // Arrange
        Crumb breadcrumb = new("title", "slug", "type", new List<string>());
        _crumbFactory
            .Setup(crumb => crumb.ToModel(_contentfulTopic.Breadcrumbs.First()))
            .Returns(breadcrumb);

        SubItem subItem = new("slug1",
                            "title",
                            "teaser",
                            "teaser image",
                            "icon",
                            "type",
                            DateTime.MinValue,
                            DateTime.MaxValue,
                            "image",
                            new List<SubItem>(),
                            EColourScheme.Blue,
                            new List<string>());
        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(_contentfulTopic.SubItems.First()))
            .Returns(subItem);

        SubItem secondaryItem = new("slug2",
                                    "title",
                                    "teaser",
                                    "teaser image",
                                    "icon",
                                    "type",
                                    DateTime.MinValue,
                                    DateTime.MaxValue,
                                    "image",
                                    new List<SubItem>(),
                                    EColourScheme.Blue,
                                    new List<string>());
        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(_contentfulTopic.SecondaryItems.First()))
            .Returns(secondaryItem);

        SubItem tertiaryItem = new("slug3",
                                "title",
                                "teaser",
                                "teaser image",
                                "icon",
                                "type",
                                DateTime.MinValue,
                                DateTime.MaxValue,
                                "image",
                                new List<SubItem>(),
                                EColourScheme.Blue,
                                new List<string>());

        CallToActionBanner callToAction = new()
        {
            AltText = "altText",
            ButtonText = "buttonText",
            Colour = EColourScheme.Pink,
            Image = "image",
            Link = "link",
            Teaser = "teaser",
            Title = "title"
        };
        
        _callToActionFactory
            .Setup(callToActionFactory => callToActionFactory.ToModel(_contentfulTopic.CallToAction))
            .Returns(callToAction);

        EventBanner eventBanner = new("Title", "Teaser", "Icon", "Link", EColourScheme.Green);
        _eventBannerFactory
            .Setup(eventBannerFactory => eventBannerFactory.ToModel(_contentfulTopic.EventBanner))
            .Returns(eventBanner);

        Alert alert = new("title",
                        "body",
                        "test",
                        new DateTime(2017, 01, 01),
                        new DateTime(2017, 04, 10),
                        string.Empty,
                        false,
                        string.Empty,
                        new List<string>());
        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(_contentfulTopic.Alerts.First()))
            .Returns(alert);

        CarouselContent carouselContent = new("title",
                                            "slug",
                                            "teaser",
                                            "image",
                                            DateTime.Now.AddDays(-1),
                                            DateTime.Now.AddDays(2),
                                            "url");
        _carouselFactory
            .Setup(carouselFactory => carouselFactory.ToModel(It.IsAny<ContentfulCarouselContent>()))
            .Returns(carouselContent);

        // Act
        Topic result = _topicFactory.ToModel(_contentfulTopic);

        // Assert
        Assert.Single(result.SubItems);
        Assert.Equal(subItem, result.SubItems.First());
        Assert.Single(result.SecondaryItems);
        Assert.Equal(secondaryItem, result.SecondaryItems.First());
        Assert.Equal(callToAction, result.CallToAction);
        Assert.Equal(eventBanner, result.EventBanner);
        Assert.Single(result.Alerts);
        Assert.Equal(alert, result.Alerts.First());
        Assert.Equal("background-image-url.jpg", result.BackgroundImage);
        Assert.Single(result.Breadcrumbs);
        Assert.Equal(breadcrumb, result.Breadcrumbs.First());
        Assert.Equal("icon", result.Icon);
        Assert.Equal("background-image-url.jpg", result.Image);
        Assert.Equal("slug", result.Slug);
        Assert.Equal("name", result.Title);
        Assert.Equal("summary", result.Summary);
        Assert.Equal(DateTime.MinValue, result.SunriseDate);
        Assert.Equal(DateTime.MaxValue, result.SunsetDate);
        Assert.Equal("teaser", result.Teaser);
        Assert.Equal("metaDescription", result.MetaDescription);
        Assert.False(result.DisplayContactUs);
        Assert.NotNull(result.CallToAction);
    }

    [Fact]
    public void ToModel_ShouldNotAddBreadcrumbsOrSubItemsOrSecondaryItemsOrImageOrAlerts_If_TheyAreLinks()
    {
        // Arrange
        _contentfulTopic.Breadcrumbs.First().Sys.LinkType = "Link";
        _contentfulTopic.SubItems.First().Sys.LinkType = "Link";
        _contentfulTopic.SecondaryItems.First().Sys.LinkType = "Link";
        _contentfulTopic.Alerts.First().Sys.LinkType = "Link";
        _contentfulTopic.BackgroundImage.SystemProperties.LinkType = "Link";

        // Act
        Topic topic = _topicFactory.ToModel(_contentfulTopic);

        // Assert
        Assert.Empty(topic.Breadcrumbs);
        Assert.Empty(topic.SubItems);
        Assert.Empty(topic.SecondaryItems);
        Assert.Empty(topic.BackgroundImage);
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(_contentfulTopic.Breadcrumbs.First()), Times.Never);
        _subItemFactory.Verify(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithOneOutsideOfDates()
    {
        // Arrange
        List<ContentfulAlert> alerts = new() {
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 04, 10)).WithSunriseDate(new DateTime(2017, 01, 01)).Build(),
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 01, 04)).WithSunriseDate(new DateTime(2017, 01, 01)).Build()
        };

        ContentfulTopic contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

        // Act
        Topic topic = _topicFactory.ToModel(contentfulTopic);

        // Arrange
        Assert.Single(topic.Alerts);
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopic_And_FilterAlertsWithSeverityOfCondolence()
    {
        // Arrange
        List<ContentfulAlert> contentfulAlerts = new() {
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 04, 10)).WithSunriseDate(new DateTime(2017, 01, 01)).WithSeverity("Information").Build(),
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 04, 10)).WithSunriseDate(new DateTime(2017, 01, 01)).WithSeverity("Condolence").Build()
        };

        ContentfulTopic contentfulTopic = new ContentfulTopicBuilder().Alerts(contentfulAlerts).Build();

        // Act
        Topic topic = _topicFactory.ToModel(contentfulTopic);

        // Assert
        Assert.Single(topic.Alerts);
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopic_And_FilterAlertsWithAllInsideOfDates()
    {
        // Arrange
        List<ContentfulAlert> alerts = new() {
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 04, 10)).WithSunriseDate(new DateTime(2017, 01, 01)).Build(),
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 02, 03)).WithSunriseDate(new DateTime(2017, 01, 01)).Build()
        };

        ContentfulTopic contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

        // Act
        Topic topic = _topicFactory.ToModel(contentfulTopic);

        // Assert
        Assert.Equal(2, topic.Alerts.Count());
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopic_And_FilterAlertsWithAllOutsideOfDates()
    {
        // Arrange
        List<ContentfulAlert> alerts = new() {
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 04, 10)).WithSunriseDate(new DateTime(2017, 03, 01)).Build(),
            new ContentfulAlertBuilder().WithSunsetDate(new DateTime(2017, 10, 03)).WithSunriseDate(new DateTime(2017, 03, 01)).Build()
        };

        ContentfulTopic contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

        // Act
        Topic topic = _topicFactory.ToModel(contentfulTopic);

        // Assert
        Assert.Empty(topic.Alerts);
    }
}