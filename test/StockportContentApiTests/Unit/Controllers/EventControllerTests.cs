using AutoMapper;

namespace StockportContentApiTests.Unit.Controllers;

public class EventControllerTests
{
    private readonly Mock<Func<string, string, IEventRepository>> _eventRepository = new();
    private readonly Mock<Func<string, string, IEventCategoryRepository>> _categoryRepository = new();
    private readonly Mock<IMapper> _mapper = new();
    private readonly Mock<ILogger<EventController>> _eventLogger = new();
    private readonly EventController _controller;

    public EventControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _controller = new EventController(
            new(logger.Object),
            _eventRepository.Object,
            _categoryRepository.Object,
            null,
            _mapper.Object,
            _eventLogger.Object
        );
    }

    [Fact]
    public async Task GetEventCategories_ReturnsCategories()
    {
        // Arrange
        _categoryRepository
            .Setup(repo => repo(It.IsAny<string>(), It.IsAny<string>()).GetEventCategories())
            .ReturnsAsync(HttpResponse.Successful(new EventCategory("category name", "category-slug", "category icon", "category image")));

        // Act
        IActionResult result = await _controller.GetEventCategories("test-business");

        // Assert
        _categoryRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Homepage_CallsCategoryAndEventRepositories()
    {
        // Arrange
        _categoryRepository
            .Setup(repo => repo(It.IsAny<string>(), It.IsAny<string>()).GetEventCategories())
            .ReturnsAsync(HttpResponse.Successful(new List<EventCategory> { new("category name", "category-slug", "category icon", "category image") }));

        _eventRepository
            .Setup(repo => repo(It.IsAny<string>(), It.IsAny<string>()).GetEventHomepage(It.IsAny<int>()))
            .ReturnsAsync(HttpResponse.Successful(new EventHomepage(new List<EventHomepageRow>() { new() })));

        // Act
        IActionResult result = await _controller.Homepage("test-business");

        // Assert
        _categoryRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        _eventRepository.Verify(factory => factory(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
    }
}