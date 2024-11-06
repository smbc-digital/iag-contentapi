using Microsoft.Extensions.Options;
using StockportContentApi.Controllers;

namespace StockportContentApi.Tests.Controllers;

public class ArticleControllerTests
{
    private readonly Mock<Func<string, IArticleRepository>> _mockCreateRepository = new();
    private readonly Mock<IArticleRepository> _articleRepository = new();
    private readonly Mock<IResponseHandler> _mockHandler = new();
    private readonly Mock<IOptions<RedisExpiryConfiguration>> _mockOptions = new();
    private readonly ArticleController _controller;

    public ArticleControllerTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _mockOptions
            .Setup(options => options.Value)
            .Returns(new RedisExpiryConfiguration { Articles = 60 });

        _articleRepository
            .Setup(repo => repo.GetArticle(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(new Article() { Slug = "test-article", Title = "Test article" }));

        _mockCreateRepository
            .Setup(func => func(It.IsAny<string>()))
            .Returns(_articleRepository.Object);

        _mockHandler
            .Setup(handler => handler.Get(It.IsAny<Func<Task<HttpResponse>>>()))
            .ReturnsAsync(new OkObjectResult(new Article()));

        _controller = new ArticleController(_mockHandler.Object, _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetArticle_ReturnsArticle_WhenArticleExists()
    {
        // Act
        IActionResult result = await _controller.GetArticle("test-slug", "test-business");

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task GetArticle_ReturnsNotFound_WhenArticleDoesNotExist()
    {
        // Arrange
        _mockHandler.Setup(handler => handler.Get(It.IsAny<Func<Task<HttpResponse>>>()))
                    .ReturnsAsync(new NotFoundObjectResult(null));

        // Act
        IActionResult result = await _controller.GetArticle("test-slug", "test-business");

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        _articleRepository.Verify(repo => repo.GetArticle("test-slug"), Times.Never);
    }

    [Fact]
    public async Task Index_ReturnsArticle_WhenArticleExists()
    {
        // Arrange
        _mockHandler.Setup(handler => handler.Get(It.IsAny<Func<Task<HttpResponse>>>()))
                    .ReturnsAsync(new OkObjectResult(new Article()));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    [Fact]
    public async Task Index_ReturnsNotFound_WhenArticleDoesNotExist()
    {
        // Arrange
        _mockHandler.Setup(handler => handler.Get(It.IsAny<Func<Task<HttpResponse>>>()))
                    .ReturnsAsync(new NotFoundObjectResult(null));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
        _articleRepository.Verify(repo => repo.GetArticle("test-slug"), Times.Never);
    }
}