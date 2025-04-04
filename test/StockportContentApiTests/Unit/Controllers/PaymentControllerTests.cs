using StockportContentApi.Controllers;

namespace StockportContentApiTests.Unit.Controllers;

public class PaymentControllerTests
{
    private readonly Mock<Func<string, IPaymentRepository>> _mockCreateRepository = new();
    private readonly Mock<IPaymentRepository> _mockRepository = new();
    private readonly PaymentController _controller;

    public PaymentControllerTests()
    {
        Mock<ILogger<ResponseHandler>> mockLogger = new();

        _mockCreateRepository
            .Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new PaymentController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetPayment_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Payment payment = new("title",
                            "slug",
                            "teaser",
                            "description",
                            "default",
                            "paymentDetails",
                            "reference label",
                            "fund",
                            "gl code cost centre number",
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
            .ReturnsAsync(HttpResponse.Successful(payment));

        // Act
        IActionResult result = await _controller.GetPayment("slug", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Payment payment = new("title",
                            "slug",
                            "teaser",
                            "description",
                            "default",
                            "paymentDetails",
                            "reference label",
                            "fund",
                            "gl code cost centre number",
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
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(payment));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}