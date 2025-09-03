namespace StockportContentApiTests.Unit.Controllers;

public class ContactUsIdControllerTests
{
    private readonly Mock<Func<string, IContactUsIdRepository>> _createRepository = new();
    private readonly Mock<IContactUsIdRepository> _repository = new();
    private readonly ContactUsIdController _controller;

    public ContactUsIdControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new ContactUsIdController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetContactUsIds_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        ContactUsId contactUsArea = new("name", "slug", "email address");

        _repository
            .Setup(repo => repo.GetContactUsIds(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(contactUsArea));

        // Act
        IActionResult result = await _controller.Detail("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}