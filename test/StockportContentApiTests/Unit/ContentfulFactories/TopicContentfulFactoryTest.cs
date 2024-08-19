namespace StockportContentApiTests.Unit.ContentfulFactories;

public class TopicContentfulFactoryTest
{
    private readonly ContentfulTopic _contentfulTopic;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory;
    private readonly Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>> _carouselContentFactory;
    private readonly TopicContentfulFactory _topicContentfulFactory;
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>> _topicBrandingFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();

    public TopicContentfulFactoryTest()
    {
        _contentfulTopic = new ContentfulTopicBuilder().Build();
        _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
        _subItemFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _eventBannerFactory = new Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>>();
        _carouselContentFactory = new Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>();
        _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 02, 02));
        _callToActionFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(new CallToActionBanner());
        _topicContentfulFactory = new TopicContentfulFactory(
            _subItemFactory.Object,
            _crumbFactory.Object,
            _alertFactory.Object,
            _eventBannerFactory.Object,
            _carouselContentFactory.Object,
            _timeProvider.Object,
            _callToActionFactory.Object,
            _topicBrandingFactory.Object
            );
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopic()
    {
        // Arrange
        Crumb crumb = new("title", "slug", "type");
        _crumbFactory.Setup(_ => _.ToModel(_contentfulTopic.Breadcrumbs.First())).Returns(crumb);

        SubItem subItem = new("slug1", "title", "teaser", "icon", "type", "contentType", DateTime.MinValue, DateTime.MaxValue, "image", 111, "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue);
        _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.SubItems.First())).Returns(subItem);

        SubItem secondaryItem = new("slug2", "title", "teaser", "icon", "type", "contentType", DateTime.MinValue, DateTime.MaxValue, "image", 111, "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue);
        _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.SecondaryItems.First())).Returns(secondaryItem);

        SubItem tertiaryItem = new("slug3", "title", "teaser", "icon", "type", "contentType", DateTime.MinValue, DateTime.MaxValue, "image", 111, "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue);

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
        _callToActionFactory.Setup(_ => _.ToModel(_contentfulTopic.CallToAction)).Returns(callToAction);

        EventBanner eventBanner = new("Title", "Teaser", "Icon", "Link", EColourScheme.Orange);
        _eventBannerFactory.Setup(_ => _.ToModel(_contentfulTopic.EventBanner)).Returns(eventBanner);

        Alert alert = new("title", "subheading", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10), string.Empty, false, string.Empty);
        _alertFactory.Setup(_ => _.ToModel(_contentfulTopic.Alerts.First())).Returns(alert);

        CarouselContent carouselContent = new("title", "slug", "teaser", "image", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(2), "url");
        _carouselContentFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulCarouselContent>()))
            .Returns(carouselContent);

        // Act
        Topic result = _topicContentfulFactory.ToModel(_contentfulTopic);

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
        Assert.Equal(crumb, result.Breadcrumbs.First());
        Assert.False(result.EmailAlerts);
        Assert.Equal("id", result.EmailAlertsTopicId);
        Assert.Equal("icon", result.Icon);
        Assert.Equal("background-image-url.jpg", result.Image);
        Assert.Equal("slug", result.Slug);
        Assert.Equal("name", result.Name);
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
        Topic topic = _topicContentfulFactory.ToModel(_contentfulTopic);

        // Assert
        Assert.Empty(topic.Breadcrumbs);
        Assert.Empty(topic.SubItems);
        Assert.Empty(topic.SecondaryItems);
        Assert.Empty(topic.BackgroundImage);
        _crumbFactory.Verify(_ => _.ToModel(_contentfulTopic.Breadcrumbs.First()), Times.Never);
        _subItemFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
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
        Topic topic = _topicContentfulFactory.ToModel(contentfulTopic);

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
        Topic topic = _topicContentfulFactory.ToModel(contentfulTopic);

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
        Topic topic = _topicContentfulFactory.ToModel(contentfulTopic);

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
        Topic topic = _topicContentfulFactory.ToModel(contentfulTopic);

        // Assert
        Assert.Empty(topic.Alerts);
    }
}