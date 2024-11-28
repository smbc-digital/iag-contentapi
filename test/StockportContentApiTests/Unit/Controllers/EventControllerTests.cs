using System.Net.Http.Headers;
using AutoMapper;
using Castle.Components.DictionaryAdapter;
using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class EventControllerTests
{
    private readonly Mock<Func<string, ContentfulConfig>> _mockCreateConfig;
    private readonly Mock<Func<string, CacheKeyConfig>> _mockCreateCacheKeyConfig;
    private readonly Mock<Func<ContentfulConfig, CacheKeyConfig, IEventRepository>> _mockEventRepository;
    private readonly Mock<Func<ContentfulConfig, CacheKeyConfig, IEventCategoryRepository>> _mockCategoryRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ILogger<EventController>> _mockLogger;

    private readonly EventController _controller;

    public EventControllerTests()
    {
        _mockCreateConfig = new Mock<Func<string, ContentfulConfig>>();
        _mockCreateCacheKeyConfig = new Mock<Func<string, CacheKeyConfig>>();
        _mockEventRepository = new Mock<Func<ContentfulConfig, CacheKeyConfig, IEventRepository>>();
        _mockCategoryRepository = new Mock<Func<ContentfulConfig, CacheKeyConfig, IEventCategoryRepository>>();
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockMapper = new Mock<IMapper>();
        _mockLogger = new Mock<ILogger<EventController>>();

        _controller = new EventController(
            new(mockLogger.Object),
            _mockCreateConfig.Object,
            _mockCreateCacheKeyConfig.Object,
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
            .Setup(repo => repo(It.IsAny<ContentfulConfig>(), It.IsAny<CacheKeyConfig>()).GetEventCategories())
            .ReturnsAsync(HttpResponse.Successful(new EventCategory("category name", "category-slug", "category icon")));

        // Act
        var result = await _controller.GetEventCategories("test-business");

        // Assert
        _mockCategoryRepository.Verify(factory => factory(It.IsAny<ContentfulConfig>(), It.IsAny<CacheKeyConfig>()), Times.Once);
    }

    [Fact]
    public async Task Homepage_CallsCategoryAndEventRepositories()
    {
        // Arrange
        _mockCategoryRepository
            .Setup(repo => repo(It.IsAny<ContentfulConfig>(), It.IsAny<CacheKeyConfig>()).GetEventCategories())
            .ReturnsAsync(HttpResponse.Successful(new List<EventCategory> { new("category name", "category-slug", "category icon") }));

        _mockEventRepository
            .Setup(repo => repo(It.IsAny<ContentfulConfig>(), It.IsAny<CacheKeyConfig>()).GetEventHomepage(It.IsAny<int>()))
            .ReturnsAsync(HttpResponse.Successful(new EventHomepage(new List<EventHomepageRow>() { new() })));


        // Act
        IActionResult result = await _controller.Homepage("test-business");

        // Assert
        _mockCategoryRepository.Verify(factory => factory(It.IsAny<ContentfulConfig>(), It.IsAny<CacheKeyConfig>()), Times.Once);
        _mockEventRepository.Verify(factory => factory(It.IsAny<ContentfulConfig>(), It.IsAny<CacheKeyConfig>()), Times.Once);

    }
}