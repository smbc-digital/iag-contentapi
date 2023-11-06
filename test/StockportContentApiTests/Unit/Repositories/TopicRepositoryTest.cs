﻿namespace StockportContentApiTests.Unit.Repositories;

public class TopicRepositoryTest
{
    private readonly TopicRepository _repository;
    private readonly Topic _topic;
    private readonly Topic _topicWithAlertsOutsideSunsetDate;
    private readonly Topic _topicWithAlertsInsideSunsetDate;
    private readonly Mock<IContentfulFactory<ContentfulTopic, Topic>> _topicFactory;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>> _topicSiteMapFactory;

    public TopicRepositoryTest()
    {
        var config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Build();

        _topic = new Topic("slug", "name", "teaser", "metaDescription", "summary", "icon", "backgroundImage", "image", new List<SubItem>(), new List<SubItem>(),
            new List<SubItem>(), new List<Crumb>(), new List<Alert>(), DateTime.MinValue, DateTime.MinValue, true, "test-id", new NullEventBanner(), "expandingLinkTitle", new CarouselContent(), "eventCategory", null, new CallToActionBanner(), null, string.Empty, new List<ExpandingLinkBox>());

        var alertOutside = new Alert("title", "subheading", "body", "warning", new DateTime(2017, 01, 01),
            new DateTime(2017, 01, 02), string.Empty, false, string.Empty);
        var alertInside = new Alert("title", "subheading", "body", "warning", new DateTime(2017, 01, 01),
            new DateTime(2017, 02, 03), string.Empty, false, string.Empty);

        _topicWithAlertsOutsideSunsetDate = new Topic("slug", "name", "teaser", "metaDescription", "summary", "icon", "backgroundImage", "image",
            new List<SubItem>(), new List<SubItem>(), new List<SubItem>(), new List<Crumb>(), new List<Alert> { alertOutside },
            DateTime.MinValue, DateTime.MinValue, true, "test-id", new NullEventBanner(), "expandingLinkTitle", new CarouselContent(), "eventCategory", null, new CallToActionBanner(), null, string.Empty, new List<ExpandingLinkBox>());

        _topicWithAlertsInsideSunsetDate = new Topic("slug", "name", "teaser", "metaDescription", "summary", "icon", "backgroundImage", "image",
            new List<SubItem>(), new List<SubItem>(), new List<SubItem>(), new List<Crumb>(), new List<Alert> { alertOutside, alertInside },
            DateTime.MinValue, DateTime.MinValue, true, "test-id", new NullEventBanner(), "expandingLinkTitle", new CarouselContent(), "eventCategory", null, new CallToActionBanner(), null, string.Empty, new List<ExpandingLinkBox>());

        _topicFactory = new Mock<IContentfulFactory<ContentfulTopic, Topic>>();
        _topicSiteMapFactory = new Mock<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>>();
        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _repository = new TopicRepository(config, contentfulClientManager.Object, _topicFactory.Object, _topicSiteMapFactory.Object);
    }

    [Fact]
    public void GetTopicByTopicSlug_ShouldReturnSuccessfulStatus()
    {
        // Arrange
        const string slug = "a-slug";
        var contentfulTopic = new ContentfulTopicBuilder().Slug(slug).Build();
        ContentfulCollection<ContentfulTopic> collection = new()
        {
            Items = new List<ContentfulTopic> { contentfulTopic }
        };

        var builder = new QueryBuilder<ContentfulTopic>().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(2);
        _contentfulClient.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulTopic>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _topicFactory.Setup(_ => _.ToModel(contentfulTopic)).Returns(_topic);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetTopicByTopicSlug(slug));

        // Arrange
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<Topic>(), _topic);
    }

    [Fact]
    public void GetTopicByTopicSlug_ShouldReturnNotFound_If_TopicDoesNotExist()
    {
        // Arrange
        const string slug = "not-found";

        ContentfulCollection<ContentfulTopic> collection = new()
        {
            Items = new List<ContentfulTopic>()
        };

        var builder = new QueryBuilder<ContentfulTopic>().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(1);
        _contentfulClient.Setup(o => o.GetEntries(It.IsAny<QueryBuilder<ContentfulTopic>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.GetTopicByTopicSlug(slug));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"No topic found for '{slug}'", response.Error);
    }

    [Fact]
    public void Get_ShouldReturnSuccessfulStatusCode(){
        // Arrange
        const string slug = "a-slug";

        List<ContentfulSectionForSiteMap> sections = new() {
            new ContentfulSectionForSiteMap() {
                Slug = "section-slug",
                SunriseDate = new DateTime(),
                SunsetDate = new DateTime()
            }
        };

        var contentfulTopic = new ContentfulTopicForSiteMapBuilder()
            .Slug(slug)
            .Sections(sections)
            .Build();
        
        ContentfulCollection<ContentfulTopicForSiteMap> collection = new()
        {
            Items = new List<ContentfulTopicForSiteMap> { contentfulTopic }
        };

        var builder = new QueryBuilder<ContentfulTopicForSiteMap>().ContentTypeIs("topic").Include(2);
        _contentfulClient.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulTopicForSiteMap>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get());

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public void Get_ShouldReturnFailureStatusCode_IfNoEntries(){
        // Arrange
        ContentfulCollection<ContentfulTopicForSiteMap> collection = new()
        {
            Items = new List<ContentfulTopicForSiteMap>()
        };

        var builder = new QueryBuilder<ContentfulTopicForSiteMap>().ContentTypeIs("topic").Include(2);
        _contentfulClient.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulTopicForSiteMap>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        var response = AsyncTestHelper.Resolve(_repository.Get());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}