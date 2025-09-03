using System.Threading.Tasks;

namespace StockportContentApiTests.Unit.Repositories;

public class TopicRepositoryTests
{
    private readonly TopicRepository _repository;
    private readonly Topic _topic;
    private readonly Mock<IContentfulFactory<ContentfulTopic, Topic>> _topicFactory = new();
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap>> _topicSiteMapFactory = new();

    public TopicRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _topic = new Topic("slug",
                        "name",
                        "teaser",
                        "metaDescription",
                        "summary",
                        "icon",
                        "backgroundImage",
                        "image",
                        new List<SubItem>(),
                        new List<SubItem>(),
                        new List<SubItem>(),
                        new List<Crumb>(),
                        new List<Alert>(),
                        DateTime.MinValue,
                        DateTime.MinValue,
                        new NullEventBanner(),
                        new CarouselContent(),
                        "eventCategory",
                        new CallToActionBanner(),
                        null,
                        string.Empty);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _repository = new TopicRepository(config, contentfulClientManager.Object, _topicFactory.Object, _topicSiteMapFactory.Object);
    }

    [Fact]
    public async Task GetTopicByTopicSlug_ShouldReturnSuccessfulStatus()
    {
        // Arrange
        ContentfulTopic contentfulTopic = new ContentfulTopicBuilder().Slug("a-slug").Build();
        ContentfulCollection<ContentfulTopic> collection = new()
        {
            Items = new List<ContentfulTopic> { contentfulTopic }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulTopic>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _topicFactory
            .Setup(topicFactory => topicFactory.ToModel(contentfulTopic))
            .Returns(_topic);

        // Act
        HttpResponse response = await _repository.GetTopicByTopicSlug("a-slug");

        // Arrange
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<Topic>(), _topic);
    }

    [Fact]
    public async Task GetTopicByTopicSlug_ShouldReturnNotFound_If_TopicDoesNotExist()
    {
        // Arrange
        ContentfulCollection<ContentfulTopic> collection = new()
        {
            Items = new List<ContentfulTopic>()
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulTopic>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.GetTopicByTopicSlug("not-found");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal($"No topic found for 'not-found'", response.Error);
    }

    [Fact]
    public async Task Get_ShouldReturnSuccessfulStatusCode()
    {
        // Arrange
        List<ContentfulSectionForSiteMap> sections = new() {
            new ContentfulSectionForSiteMap() {
                Slug = "section-slug",
                SunriseDate = new DateTime(),
                SunsetDate = new DateTime()
            }
        };

        ContentfulTopicForSiteMap contentfulTopic = new ContentfulTopicForSiteMapBuilder()
            .WithSlug("a-slug")
            .WithSections(sections)
            .Build();

        ContentfulCollection<ContentfulTopicForSiteMap> collection = new()
        {
            Items = new List<ContentfulTopicForSiteMap> { contentfulTopic }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulTopicForSiteMap>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnFailureStatusCode_IfNoEntries()
    {
        // Arrange
        ContentfulCollection<ContentfulTopicForSiteMap> collection = new()
        {
            Items = new List<ContentfulTopicForSiteMap>()
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulTopicForSiteMap>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}