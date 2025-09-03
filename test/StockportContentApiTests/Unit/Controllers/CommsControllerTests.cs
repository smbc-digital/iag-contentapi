namespace StockportContentApiTests.Unit.Controllers;

public class CommsControllerTests
{
    private readonly Mock<Func<string, ICommsRepository>> _createRepository = new();
    private readonly Mock<ICommsRepository> _repository = new();
    private readonly CommsController _controller;

    public CommsControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new CommsController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task Get_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        CommsHomepage commsHomepage = new("title",
                                        "meta description",
                                        "latest news header",
                                        "twitter feed header",
                                        "instagram feed title",
                                        "instagram feed url",
                                        "facebook feed title",
                                        new List<BasicLink>(),
                                        new Event("title",
                                                "slug",
                                                "teaser",
                                                "image",
                                                "description",
                                                "10",
                                                "location",
                                                "submitted by",
                                                new DateTime(),
                                                "10:00",
                                                "11:30",
                                                5,
                                                EventFrequency.None,
                                                new List<Crumb>(),
                                                "thumbnail image url",
                                                new List<Document>(),
                                                new MapPosition(),
                                                false,
                                                "booking information",
                                                new DateTime(),
                                                new List<string>(),
                                                new List<Alert>(),
                                                new List<EventCategory>(),
                                                true,
                                                true,
                                                "Logo title",
                                                new List<TrustedLogo>(),
                                                "01234564567",
                                                "email",
                                                "website",
                                                "facebook",
                                                "instagram",
                                                "linked in",
                                                "meta description",
                                                "duration",
                                                "languages",
                                                new List<CallToActionBanner>()),
                                        new CallToActionBanner(),
                                        "email alerts topic id");

        _repository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(commsHomepage));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}