namespace StockportContentApiTests.Unit.Controllers;

public class ContactUsControllerTests
{
    private readonly Mock<Func<string, IContactUsAreaRepository>> _createRepository = new();
    private readonly Mock<IContactUsAreaRepository> _repository = new();
    private readonly ContactUsController _controller;

    public ContactUsControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new ContactUsController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetContactUsArea_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        ContactUsArea contactUsArea = new("slug",
                                        "title",
                                        new List<Crumb>(),
                                        new List<Alert>(),
                                        new List<SubItem>(),
                                        new List<ContactUsCategory>(),
                                        "inset text title",
                                        "inset text body",
                                        "meta description");

        _repository
            .Setup(repo => repo.GetContactUsArea("tagId"))
            .ReturnsAsync(HttpResponse.Successful(contactUsArea));

        // Act
        IActionResult result = await _controller.GetContactUsArea("tagId");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}