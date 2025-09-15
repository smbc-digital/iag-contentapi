namespace StockportContentApiTests.Unit.Controllers;

public class ServicePayPaymentControllerTests
{
    private readonly Mock<Func<string, IServicePayPaymentRepository>> _createRepository = new();
    private readonly Mock<IServicePayPaymentRepository> _repository = new();
    private readonly ServicePayPaymentController _controller;

    public ServicePayPaymentControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetPayment_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        ServicePayPayment servicePayPayment = new("title",
                            "slug",
                            "teaser",
                            "description",
                            "paymentDetails",
                            "reference label",
                            "icon",
                            new List<Crumb>(),
                            EPaymentReferenceValidation.None,
                            "meta description",
                            "return url",
                            "catalogue id",
                            "account reference",
                            "payment description",
                            new List<Alert>());

        _repository
            .Setup(repo => repo.GetPayment(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(servicePayPayment));

        // Act
        IActionResult result = await _controller.GetPayment("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}