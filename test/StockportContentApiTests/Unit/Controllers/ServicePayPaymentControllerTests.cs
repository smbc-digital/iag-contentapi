using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class ServicePayPaymentControllerTests
{
    private readonly Mock<Func<string, IServicePayPaymentRepository>> _mockCreateRepository = new();
    private readonly Mock<IServicePayPaymentRepository> _mockRepository = new();
    private readonly ServicePayPaymentController _controller;

    public ServicePayPaymentControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new(new(mockLogger.Object), _mockCreateRepository.Object);
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

        _mockRepository
            .Setup(repo => repo.GetPayment(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(servicePayPayment));

        // Act
        IActionResult result = await _controller.GetPayment("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}