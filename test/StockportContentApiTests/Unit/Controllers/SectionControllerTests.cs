namespace StockportContentApiTests.Unit.Controllers;

public class SectionControllerTests
{
    private readonly Mock<Func<string, ISectionRepository>> _createRepository = new();
    private readonly Mock<ISectionRepository> _repository = new();
    private readonly SectionController _controller;

    public SectionControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new SectionController(new(logger.Object), _createRepository.Object);
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
                            new List<Alert>(),
                            new List<InlineQuote>());

        _repository
            .Setup(repo => repo.GetSections(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(section));

        // Act
        IActionResult result = await _controller.GetSection("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
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

        _repository
            .Setup(repo => repo.Get("tagId"))
            .ReturnsAsync(HttpResponse.Successful(sections));

        // Act
        IActionResult result = await _controller.Get("tagId");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}