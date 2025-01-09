using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class TopicControllerTests
{
    private readonly Mock<Func<string, ITopicRepository>> _mockCreateRepository = new();
    private readonly Mock<ITopicRepository> _mockRepository = new();
    private readonly TopicController _controller;

    public TopicControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new TopicController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetTopicByTopicSlug_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Topic topic = new("title",
                        "slug",
                        new List<SubItem>(),
                        new List<SubItem>());

        _mockRepository
            .Setup(repo => repo.GetTopicByTopicSlug(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(topic));

        // Act
        IActionResult result = await _controller.GetTopicByTopicSlug("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<Topic> topics = new()
        {
            new Topic("title",
                "slug",
                new List<SubItem>(),
                new List<SubItem>()),
            new Topic("title",
                "slug",
                new List<SubItem>(),
                new List<SubItem>())
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(topics));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}