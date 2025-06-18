using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class SectionControllerTests
{
    private readonly Mock<Func<string, ISectionRepository>> _mockCreateRepository = new();
    private readonly Mock<ISectionRepository> _mockRepository = new();
    private readonly SectionController _controller;

    public SectionControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new SectionController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetSection_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Section section = new("title",
                            "slug",
                            "meta description",
                            "body",
                            new List<Profile>(),
                            new List<Document>(),
                            "logo area title",
                            new List<TrustedLogo>(),
                            new DateTime(),
                            new DateTime(),
                            new DateTime(),
                            new List<Alert>());

        _mockRepository
            .Setup(repo => repo.GetSections(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(section));

        // Act
        IActionResult result = await _controller.GetSection("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<ContentfulSectionForSiteMap> sections = new()
        {
            new ContentfulSectionForSiteMap()
            {
                Slug = "profile-one"
            },
            new ContentfulSectionForSiteMap()
            {
                Slug = "profile-two"
            },
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(sections));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}