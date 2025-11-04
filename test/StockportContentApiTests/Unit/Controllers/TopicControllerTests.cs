namespace StockportContentApiTests.Unit.Controllers;

public class TopicControllerTests
{
    private readonly Mock<Func<string, ITopicRepository>> _createRepository = new();
    private readonly Mock<ITopicRepository> _repository = new();
    private readonly TopicController _controller;

    public TopicControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new TopicController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetTopicByTopicSlug_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Topic topic = new("title",
                        "slug",
                        new List<SubItem>(),
                        new List<SubItem>());

        _repository
            .Setup(repo => repo.GetTopicByTopicSlug(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(topic));

        // Act
        IActionResult result = await _controller.GetTopicByTopicSlug("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
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

        _repository
            .Setup(repo => repo.Get("tagId"))
            .ReturnsAsync(HttpResponse.Successful(topics));

        // Act
        IActionResult result = await _controller.Get("tagId");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}