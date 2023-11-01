﻿namespace StockportContentApiTests.Unit.ContentfulFactories;

public class TopicContentfulFactoryTest
{
    private readonly ContentfulTopic _contentfulTopic;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory;
    private readonly Mock<IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>> _expandingLinkBoxFactory;
    private readonly Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>> _carouselContentFactory;
    private readonly TopicContentfulFactory _topicContentfulFactory;
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionBannerFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>> _topicBrandingFactory = new();

    public TopicContentfulFactoryTest()
    {
        _contentfulTopic = new ContentfulTopicBuilder().Build();
        _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
        _subItemFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _eventBannerFactory = new Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>>();
        _expandingLinkBoxFactory = new Mock<IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>>();
        _carouselContentFactory = new Mock<IContentfulFactory<ContentfulCarouselContent, CarouselContent>>();
        _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 02, 02));
        _callToActionBannerFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(new CallToActionBanner());
        _topicContentfulFactory = new TopicContentfulFactory(
            _subItemFactory.Object,
            _crumbFactory.Object,
            _alertFactory.Object,
            _eventBannerFactory.Object,
            _expandingLinkBoxFactory.Object,
            _carouselContentFactory.Object,
            _timeProvider.Object,
            _callToActionBannerFactory.Object,
            _topicBrandingFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopic()
    {
        //Arrange
        var crumb = new Crumb("title", "slug", "type");
        _crumbFactory.Setup(_ => _.ToModel(_contentfulTopic.Breadcrumbs.First())).Returns(crumb);

        var subItem = new SubItem("slug1", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
        _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.SubItems.First())).Returns(subItem);

        var secondaryItem = new SubItem("slug2", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
        _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.SecondaryItems.First())).Returns(secondaryItem);

        var tertiaryItem = new SubItem("slug3", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
        _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.TertiaryItems.First())).Returns(tertiaryItem);

        var callToAction = new CallToActionBanner()
        {
            AltText = "altText",
            ButtonText = "buttonText",
            Colour = "Pink",
            Image = "image",
            Link = "link",
            Teaser = "teaser",
            Title = "title"
        };
        _callToActionBannerFactory.Setup(_ => _.ToModel(_contentfulTopic.CallToAction)).Returns(callToAction);

        var eventBanner = new EventBanner("Title", "Teaser", "Icon", "Link");
        _eventBannerFactory.Setup(_ => _.ToModel(_contentfulTopic.EventBanner)).Returns(eventBanner);

        var alert = new Alert("title", "subheading", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10), string.Empty, false, string.Empty);
        _alertFactory.Setup(_ => _.ToModel(_contentfulTopic.Alerts.First())).Returns(alert);

        var carouselContent = new CarouselContent("title", "slug", "teaser", "image", DateTime.Now.AddDays(-1), DateTime.Now.AddDays(2), "url");
        _carouselContentFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulCarouselContent>()))
            .Returns(carouselContent);

        var expandingLinkBox = new ExpandingLinkBox("title", new List<SubItem>());
        _expandingLinkBoxFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulExpandingLinkBox>()))
            .Returns(expandingLinkBox);

        //Act
        var result = _topicContentfulFactory.ToModel(_contentfulTopic);

        //Assert
        Assert.Single(result.SubItems);
        Assert.Equal(subItem, result.SubItems.First());
        Assert.Single(result.SecondaryItems);
        Assert.Equal(secondaryItem, result.SecondaryItems.First());
        Assert.Single(result.TertiaryItems);
        Assert.Equal(tertiaryItem, result.TertiaryItems.First());
        Assert.Equal(callToAction, result.CallToAction);
        Assert.Equal(eventBanner, result.EventBanner);
        Assert.Single(result.Alerts);
        Assert.Equal(alert, result.Alerts.First());
        Assert.Equal("background-image-url.jpg", result.BackgroundImage);
        Assert.Single(result.Breadcrumbs);
        Assert.Equal(crumb, result.Breadcrumbs.First());
        Assert.False(result.EmailAlerts);
        Assert.Equal("id", result.EmailAlertsTopicId);
        Assert.Single(result.ExpandingLinkBoxes);
        Assert.Equal(expandingLinkBox, result.ExpandingLinkBoxes.First());
        Assert.Equal("expandingLinkTitle", result.ExpandingLinkTitle);
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
    public void ToModel_ShouldNotAddBreadcrumbsOrSubItemsOrSecondaryItemsOrTertiaryItemsOrImageOrAlerts_If_TheyAreLinks()
    {
        // Arrange
        _contentfulTopic.Breadcrumbs.First().Sys.LinkType = "Link";
        _contentfulTopic.SubItems.First().Sys.LinkType = "Link";
        _contentfulTopic.SecondaryItems.First().Sys.LinkType = "Link";
        _contentfulTopic.TertiaryItems.First().Sys.LinkType = "Link";
        _contentfulTopic.Alerts.First().Sys.LinkType = "Link";
        _contentfulTopic.BackgroundImage.SystemProperties.LinkType = "Link";

        // Act
        var topic = _topicContentfulFactory.ToModel(_contentfulTopic);

        // Assert
        Assert.Empty(topic.Breadcrumbs);
        Assert.Empty(topic.SubItems);
        Assert.Empty(topic.SecondaryItems);
        Assert.Empty(topic.TertiaryItems);
        Assert.Empty(topic.BackgroundImage);
        _crumbFactory.Verify(_ => _.ToModel(_contentfulTopic.Breadcrumbs.First()), Times.Never);
        _subItemFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithOneOutsideOfDates()
    {
        // Arrange
        List<ContentfulAlert> alerts = new() {
            new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 01, 01)).Build(),
            new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 01, 04)).SunriseDate(new DateTime(2017, 01, 01)).Build()
        };

        var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();
        
        // Act
        var topic = _topicContentfulFactory.ToModel(contentfulTopic);

        // Arrange
        Assert.Single(topic.Alerts);
    }

    [Fact]
    public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithAllInsideOfDates()
    {
        var alerts = new List<ContentfulAlert>{
            new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 01, 01)).Build(),
            new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 02, 03)).SunriseDate(new DateTime(2017, 01, 01)).Build()
        };

        var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

        var topic = _topicContentfulFactory.ToModel(contentfulTopic);

        topic.Alerts.Should().HaveCount(2);
    }

    [Fact]
    public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithAllOutsideOfDates()
    {
        var alerts = new List<ContentfulAlert> {
            new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 03, 01)).Build(),
            new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 10, 03)).SunriseDate(new DateTime(2017, 03, 01)).Build()
        };

        var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

        var topic = _topicContentfulFactory.ToModel(contentfulTopic);

        topic.Alerts.Should().HaveCount(0);
    }
}
