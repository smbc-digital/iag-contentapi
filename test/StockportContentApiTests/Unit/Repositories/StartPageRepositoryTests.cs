using System.Threading.Tasks;

namespace StockportContentApiTests.Unit.Repositories;

public class StartPageRepositoryTests : TestingBaseClass
{
    private readonly Mock<IContentfulFactory<ContentfulStartPage, StartPage>> _startPageFactory = new();
    private readonly Mock<IContentfulClient> _client = new();
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();
    private readonly StartPageRepository _repository;

    public StartPageRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _mockTimeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2017, 08, 01));
        
        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(client => client.GetClient(config))
            .Returns(_client.Object);

        ContentfulCollection<ContentfulStartPage> collection = new()
        {
            Items = new List<ContentfulStartPage> { }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulStartPage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _repository = new(config,
                        contentfulClientManager.Object,
                        _startPageFactory.Object,
                        _mockTimeProvider.Object);
    }

    [Fact]
    public async Task GetStartPage_ReturnsOKResponseWithTheContentOfStartPage_If_ThereIsItemInTheContentResponse()
    {
        // Arrange
        List<Alert> _alerts = new()
        {
            new AlertBuilder().Build()
        };

        List<Alert> _inlineAlerts = new()
        {
            new AlertBuilder().Build()
        };

        ContentfulStartPage contentfulStartPage = new ContentfulStartPageBuilder().Slug("startpage_slug").Build();
        ContentfulCollection<ContentfulStartPage> collection = new()
        {
            Items = new List<ContentfulStartPage> { contentfulStartPage }
        };

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulStartPage>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        StartPage startPageItem = new("Start Page",
                                    "startPageSlug",
                                    "this is a teaser",
                                    "This is a summary",
                                    "An upper body",
                                    "http://start.com",
                                    "Lower body",
                                    "image.jpg",
                                    "icon",
                                    new List<Crumb> { new("title", "slug", "type") },
                                    _alerts,
                                    _inlineAlerts);

        _startPageFactory
            .Setup(startPageFactory => startPageFactory.ToModel(It.IsAny<ContentfulStartPage>()))
            .Returns(startPageItem);

        // Act
        HttpResponse response = await _repository.GetStartPage("startpage_slug");
        StartPage startPage = response.Get<StartPage>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal("Start Page", startPage.Title);
        Assert.Equal("startPageSlug", startPage.Slug);
        Assert.Equal("this is a teaser", startPage.Teaser);
        Assert.Equal("This is a summary", startPage.Summary);
        Assert.Equal("An upper body", startPage.UpperBody);
        Assert.Equal("http://start.com", startPage.FormLink);
        Assert.Equal("Lower body", startPage.LowerBody);
        Assert.Equal("image.jpg", startPage.BackgroundImage);
        Assert.Equal("icon", startPage.Icon);
        Assert.Single(startPage.Breadcrumbs);
        Assert.Equal(_alerts, startPage.Alerts);
        Assert.Equal(_inlineAlerts, startPage.AlertsInline);
    }

    [Fact]
    public async Task GetStartPage_ReturnsNotFoundResponse_If_NoItemsInTheContentResponse()
    {
        // Act
        HttpResponse response = await _repository.GetStartPage("startpage_slug");

        //Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFound_WhenNoStartPageEntriesFound()
    {
        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No start page found", response.Error);
    }
}