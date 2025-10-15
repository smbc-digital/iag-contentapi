namespace StockportContentApiTests.Unit.Controllers;

public class ArticleControllerTests
{
    private readonly Mock<Func<string, string, IArticleRepository>> _createRepository = new();
    private readonly Mock<IArticleRepository> _repository = new();
    private readonly ArticleController _controller;

    public ArticleControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new ArticleController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetArticle_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Article article = new()
        {
            Title = "Article",
            Slug = "article"
        };

        _repository
            .Setup(repo => repo.GetArticle(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(article));

        // Act
        IActionResult result = await _controller.GetArticle("article", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<ArticleSiteMap> articles = new()
        {
            new ArticleSiteMap("article", new DateTime(), new DateTime()),
            new ArticleSiteMap("article-site-map2", new DateTime().AddDays(2), new DateTime().AddDays(3)),
        };

        _repository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(articles));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}