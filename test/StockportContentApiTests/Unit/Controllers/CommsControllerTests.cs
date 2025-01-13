using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class CommsControllerTests
{
    private readonly Mock<Func<string, ICommsRepository>> _mockCreateRepository = new();
    private readonly Mock<ICommsRepository> _mockRepository = new();
    private readonly CommsController _controller;

    public CommsControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new CommsController(new(mockLogger.Object), _mockCreateRepository.Object);
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
                                                new List<string>(),
                                                new MapPosition(),
                                                false,
                                                "booking information",
                                                new DateTime(),
                                                new List<string>(),
                                                null,
                                                new List<Alert>(),
                                                new List<EventCategory>(),
                                                true,
                                                true,
                                                "accessible transport link",
                                                "Logo title",
                                                new List<GroupBranding>(),
                                                "01234564567",
                                                "email",
                                                "website",
                                                "facebook",
                                                "instagram",
                                                "linked in",
                                                "meta description",
                                                "duration",
                                                "languages"),
                                        new CallToActionBanner(),
                                        "email alerts topic id");

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(commsHomepage));

        // Act
        IActionResult result = await _controller.Get("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}