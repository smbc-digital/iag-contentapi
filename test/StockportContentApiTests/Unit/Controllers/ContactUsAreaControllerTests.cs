using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class ContactUsControllerTests
{
    private readonly Mock<Func<string, IContactUsAreaRepository>> _mockCreateRepository = new();
    private readonly Mock<IContactUsAreaRepository> _mockRepository = new();
    private readonly ContactUsController _controller;

    public ContactUsControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new ContactUsController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetContactUsArea_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        ContactUsArea contactUsArea = new("slug",
                                        "title",
                                        "categories title",
                                        new List<Crumb>(),
                                        new List<Alert>(),
                                        new List<SubItem>(),
                                        new List<ContactUsCategory>(),
                                        "inset text title",
                                        "inset text body",
                                        "meta description");

        _mockRepository
            .Setup(repo => repo.GetContactUsArea())
            .ReturnsAsync(HttpResponse.Successful(contactUsArea));

        // Act
        IActionResult result = await _controller.GetContactUsArea("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}