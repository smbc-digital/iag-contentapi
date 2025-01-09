using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class ContactUsIdControllerTests
{
    private readonly Mock<Func<string, IContactUsIdRepository>> _mockCreateRepository = new();
    private readonly Mock<IContactUsIdRepository> _mockRepository = new();
    private readonly ContactUsIdController _controller;

    public ContactUsIdControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new ContactUsIdController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetContactUsIds_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        ContactUsId contactUsArea = new("name",
                                        "slug",
                                        "email address",
                                        "success page button text",
                                        "success page return url");

        _mockRepository
            .Setup(repo => repo.GetContactUsIds(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(contactUsArea));

        // Act
        IActionResult result = await _controller.Detail("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}