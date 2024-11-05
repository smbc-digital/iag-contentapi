using Microsoft.Extensions.Options;
using StockportContentApi.Controllers;

namespace StockportContentApi.Tests.Controllers;

public class ArticleControllerTests
{
    private readonly Mock<Func<string, ArticleRepository>> _createRepository = new();
    private readonly Mock<ResponseHandler> _responseHandler = new();
    private readonly Mock<ArticleRepository> _mockArticleRepository;
    private readonly ArticleController _controller;
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory = new();
    private readonly Mock<ICache> _cache = new();
    private readonly Mock<IOptions<RedisExpiryConfiguration>> _mockOptions = new();
    private readonly FakeLogger<ResponseHandler> _logger = new();
    private readonly Mock<IConfiguration> _configuration = new();
    private readonly ContentfulConfig _config;

    public ArticleControllerTests()
    {
        _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();
        
        _responseHandler
            .Setup(handler => handler.Get(It.IsAny<Func<Task<HttpResponse>>>()))
            .Returns(async (Func<Task<HttpResponse>> func) =>
            {
                var response = await func();
                return new OkObjectResult(response);
            });


        _configuration
            .Setup(conf => conf["redisExpiryTimes:Articles"])
            .Returns("0");

        ContentfulCollection<ContentfulArticle> collection = new()
        {
            Items = new List<ContentfulArticle>()
        };

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager.Setup(_ => _.GetClient(_config)).Returns(_contentfulClient.Object);

        
        ArticleContentfulFactory contentfulFactory = new(
            _sectionFactory.Object,
            new Mock<IContentfulFactory<ContentfulReference, Crumb>>().Object,
            new Mock<IContentfulFactory<ContentfulProfile, Profile>>().Object,
            new Mock<IContentfulFactory<ContentfulArticle, Topic>>().Object,
            new DocumentContentfulFactory(),
            _videoRepository.Object,
            _mockTimeProvider.Object,
            new Mock<IContentfulFactory<ContentfulAlert, Alert>>().Object,
            new Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>().Object,
            new Mock<IContentfulFactory<ContentfulReference, SubItem>>().Object
        );

        _mockArticleRepository = new(_config,
                    contentfulClientManager.Object,
                    _mockTimeProvider.Object,
                    contentfulFactory,
                    new ArticleSiteMapContentfulFactory(),
                    _videoRepository.Object,
                    _cache.Object,
                    _mockOptions.Object);


        _controller = new ArticleController(
            _responseHandler.Object,
            _createRepository.Object
        );
    }

    // [Fact]
    // public async Task GetArticle_ShouldReturnOk_WhenArticleExists()
    // {
    //     // Arrange
    //     HttpResponseMessage mockResponse = new(HttpStatusCode.OK) 
    //     {
    //         Content = new StringContent("{\"title\":\"Test Article\"}")
    //     };

    //     _createRepository
    //         .Setup(f => f(It.IsAny<string>()))
    //         .Returns(_mockArticleRepository.Object);

    //     _mockArticleRepository
    //         .Setup(repo => repo.GetArticle("test-article"))
    //         .ReturnsAsync(HttpResponse.Successful(new()));

    //     // Act
    //     IActionResult result = await _controller.GetArticle("test-article", It.IsAny<string>());

    //     // Assert
    //     OkObjectResult okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.Equal(mockResponse, okResult.Value);
    // }


    // [Fact]
    // public async Task GetArticle_ShouldCallRepositoryGetArticle_WithCorrectParameters()
    // {
    //     // Arrange
    //     HttpResponseMessage mockResponse = new(HttpStatusCode.OK)
    //     {
    //         Content = new StringContent("{\"title\":\"Test Article\"}")
    //     };

    //     _createRepository
    //         .Setup(f => f(It.IsAny<string>()))
    //         .Returns(_mockArticleRepository.Object);
    
    //     _responseHandler
    //         .Setup(handler => handler.Get(It.IsAny<Func<Task<HttpResponse>>>()))
    //         .Returns(async (Func<Task<HttpResponseMessage>> func) => 
    //         {
    //             var response = await func();
    //             return new OkObjectResult(response);
    //         });

    //     // Act
    //     await _controller.GetArticle("test-article", It.IsAny<string>());

    //     // Assert
    //     _mockArticleRepository.Verify(repo => repo.GetArticle("test-article"), Times.Once);
    // }

    // [Fact]
    // public async Task GetArticle_ShouldCallRepositoryGetArticle_WithCorrectParameters()
    // {
    //     // Arrange
    //     var businessId = "testBusiness";
    //     var articleSlug = "test-article";
    //     var mockResponse = new HttpResponseMessage(HttpStatusCode.OK)
    //     {
    //         Content = new StringContent("{\"title\":\"Test Article\"}")
    //     };

    //     // Set up configuration and repository mocks
    //     _createRepository
    //         .Setup(f => f(It.IsAny<string>()))
    //         .Returns(_mockArticleRepository.Object);

    //     _mockArticleRepository
    //         .Setup(repo => repo.GetArticle(articleSlug))
    //         .Returns(mockResponse);

    //     // Set up the ResponseHandler to return an OkObjectResult
    //     _responseHandler
    //         .Setup(handler => handler.Get(It.IsAny<Func<Task<HttpResponse>>>()))
    //         .Returns(async (Func<Task<HttpResponseMessage>> func) => 
    //         {
    //             var response = await func();
    //             return new OkObjectResult(response);
    //         });

    //     // Act
    //     var result = await _controller.GetArticle(articleSlug, businessId);

    //     // Assert
    //     var okResult = Assert.IsType<OkObjectResult>(result);
    //     Assert.Equal(mockResponse, okResult.Value);
    //     _mockArticleRepository.Verify(repo => repo.GetArticle(articleSlug), Times.Once);
    // }


}