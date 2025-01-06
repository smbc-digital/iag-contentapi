using AutoMapper;
using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class EventControllerTests
{
    private readonly Mock<Func<string, string, IEventRepository>> _mockEventRepository = new();
    private readonly Mock<Func<string, string, IEventCategoryRepository>> _mockCategoryRepository = new();
    private readonly Mock<IMapper> _mockMapper = new();
    private readonly Mock<ILogger<EventController>> _mockLogger = new();

    private readonly EventController _controller;

    public EventControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<EventController>>();

        _controller = new EventController(
            new(mockLogger.Object),
            _mockEventRepository.Object,
            _mockCategoryRepository.Object,
            null,
            _mockMapper.Object,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task GetEventCategories_ReturnsCategories()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(repo => repo(It.IsAny<string>(), It.IsAny<string>()).GetEventCategories())
            .ReturnsAsync(HttpResponse.Successful(new EventCategory("category name", "category-slug", "category icon", "category image")));

        // Act
        IActionResult result = await _controller.GetEventCategories("test-business");

        // Assert
        _mockCategoryRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Homepage_CallsCategoryAndEventRepositories()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(repo => repo(It.IsAny<string>(), It.IsAny<string>()).GetEventCategories())
            .ReturnsAsync(HttpResponse.Successful(new List<EventCategory> { new("category name", "category-slug", "category icon", "category image") }));

        _mockEventRepository
            .Setup(repo => repo(It.IsAny<string>(), It.IsAny<string>()).GetEventHomepage(It.IsAny<int>()))
            .ReturnsAsync(HttpResponse.Successful(new EventHomepage(new List<EventHomepageRow>() { new() })));


        // Act
        IActionResult result = await _controller.Homepage("test-business");

        // Assert
        _mockCategoryRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _mockEventRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);

    }
}