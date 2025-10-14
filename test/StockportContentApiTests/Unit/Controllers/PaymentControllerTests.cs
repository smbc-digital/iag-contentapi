namespace StockportContentApiTests.Unit.Controllers;

public class PaymentControllerTests
{
    private readonly Mock<Func<string, IPaymentRepository>> _createRepository = new();
    private readonly Mock<IPaymentRepository> _repository = new();
    private readonly PaymentController _controller;

    public PaymentControllerTests()
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

        _repository
            .Setup(repo => repo.GetPayment(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(payment));

        // Act
        IActionResult result = await _controller.GetPayment("slug", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
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

        _repository
            .Setup(repo => repo.Get(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(payment));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}