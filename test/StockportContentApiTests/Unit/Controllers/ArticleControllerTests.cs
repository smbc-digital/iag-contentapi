using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class ArticleControllerTests
{
    private readonly Mock<Func<string, IArticleRepository>> _mockCreateRepository = new();
    private readonly Mock<IArticleRepository> _mockRepository = new();
    private readonly ArticleController _controller;

    public ArticleControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new ArticleController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetArticle_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Article expectedArticle = new()
        {
            Title = "Article",
            Slug = "article"
        };

        _mockRepository
            .Setup(repo => repo.GetArticle(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(new Article()));

        // Act
        IActionResult result = await _controller.GetArticle("article", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<ArticleSiteMap> expectedArticles = new()
        {
            new ArticleSiteMap("article", new DateTime(), new DateTime()),
            new ArticleSiteMap("article-site-map2", new DateTime().AddDays(2), new DateTime().AddDays(3)),
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(new List<ArticleSiteMap>()));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}