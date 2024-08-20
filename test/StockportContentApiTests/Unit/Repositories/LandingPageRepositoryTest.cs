using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Xunit.Sdk;

namespace StockportContentApiTests.Unit.Repositories;

public class LandingPageRepositoryTest
{
    private readonly LandingPageRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulLandingPage, LandingPage>> _contentfulFactory = new();

    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory = new();
    private readonly Mock<ICache> _cache;

    public LandingPageRepositoryTest()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Build();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(_ => _.GetClient(config)).Returns(_contentfulClient.Object);
        _repository = new LandingPageRepository(config, _contentfulFactory.Object, contentfulClientManager.Object, _subItemFactory.Object);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsSuccessResponse_WhenLandingPageIsFound()
    {
        // Arrange
        ContentfulLandingPage contentfulLandingPage = new() { Content = new Dictionary<string, object> { { "content", "[]" } } };
        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(new LandingPage());

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task GetLandingPage_ReturnsNotFoundResponse_WhenLandingPageIsNotFound()
    {
        // Arrange
        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = Enumerable.Empty<ContentfulLandingPage>() };
        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        // Act
        HttpResponse response = await _repository.GetLandingPage("non-existent-slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No Landing Page found", response.Error);
    }

    [Fact]
    public async Task GetContentBlock_ReturnsSubItem_WhenContentBlockIsFound()
    {
        // Arrange
        ContentfulReference contentfulReference = new();
        ContentfulCollection<ContentfulReference> contentfulCollection = new() { Items = new[] { contentfulReference } };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        SubItem subItem = new();
        _subItemFactory.Setup(factory => factory.ToModel(contentfulReference))
            .Returns(subItem);

        // Act
        SubItem result = await _repository.GetContentBlock("test-content-block-slug");

        // Assert
        Assert.NotNull(result);
        Assert.Equal(subItem, result);
    }

    [Fact]
    public async Task GetContentBlock_ReturnsNull_WhenContentBlockIsNotFound()
    {
        // Arrange
        ContentfulCollection<ContentfulReference> contentfulCollection = new() { Items = Enumerable.Empty<ContentfulReference>() };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        // Act
        SubItem result = await _repository.GetContentBlock("non-existent-content-block-slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task GetLandingPage_PopulatesContentBlocks_WhenContentItemsArePresent()
    {
        // Arrange
        List<ContentItem> contentItems = new()
        { 
            new() { Data = new Data { Target = new ContentfulReference { Slug = "content-block-1" } } },
            new() { Data = new Data { Target = new ContentfulReference { Slug = "content-block-2" } } }
        };

        ContentfulLandingPage contentfulLandingPage = new()
        {
            Content = new Dictionary<string, object> { { "content", JsonConvert.SerializeObject(contentItems) } }
        };

        SubItem subItem1 = new() { Title = "SubItem 1" };
        SubItem subItem2 = new() { Title = "SubItem 2" };

        LandingPage landingPage = new()
        {
            Slug = "landing-page-slug",
            Title = "landing page title",
            Subtitle = "landing page subtitle",
            Breadcrumbs = new List<Crumb>(),
            Alerts = new List<Alert>(),
            Teaser = "landing page teaser",
            MetaDescription = "landing page metadescription",
            Image = new MediaAsset(),
            HeaderType = "full image",
            HeaderImage = "header image",
            ContentBlocks = new List<SubItem>() { subItem1, subItem2 },
            Content = new Dictionary<string, dynamic>()
        };

        ContentfulCollection<ContentfulLandingPage> contentfulCollection = new() { Items = new[] { contentfulLandingPage } };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulLandingPage>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(contentfulCollection);

        _contentfulFactory.Setup(factory => factory.ToModel(contentfulLandingPage))
            .Returns(landingPage);

        _subItemFactory.Setup(factory => factory.ToModel(It.IsAny<ContentfulReference>())).Returns(subItem1);

        _contentfulClient.Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulReference>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulReference>
            {
                Items = new List<ContentfulReference>
                {
                    new() { Sys = new SystemProperties { Id = "content-block-1" } },
                    new() { Sys = new SystemProperties { Id = "content-block-2" } }
                }
            });

        // Act
        HttpResponse response = await _repository.GetLandingPage("test-slug");
        LandingPage responseLandingPage = response.Get<LandingPage>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(responseLandingPage.ContentBlocks);
        Assert.Equal(2, responseLandingPage.ContentBlocks.Count());
        Assert.Equal(subItem1, responseLandingPage.ContentBlocks.ToList()[0]);
        Assert.Equal(subItem2, responseLandingPage.ContentBlocks.ToList()[1]);
    }
}