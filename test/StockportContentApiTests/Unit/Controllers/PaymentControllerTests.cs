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

        _mockCreateRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_mockRepository.Object);

        _controller = new PaymentController(new(mockLogger.Object), _mockCreateRepository.Object);
    }

    [Fact]
    public async Task GetPayment_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        Article article = new()
        {
            Title = "Article",
            Slug = "article"
        };

        _mockRepository
            .Setup(repo => repo.GetPayment(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(article));

        // Act
        IActionResult result = await _controller.GetPayment("article", "test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task Index_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<Payment> payments = new()
        {
            new Payment("title",
                        "slug",
                        "teaser",
                        "description",
                        "payment details text",
                        "reference label",
                        "paris reference",
                        "fund",
                        string.Empty,
                        "icon",
                        new List<Crumb>(),
                        EPaymentReferenceValidation.None,
                        "meta description",
                        "return url",
                        "catalogue id",
                        "account reference",
                        "payment description",
                        new List<Alert>()
            ),
            new Payment("title two",
                        "slug-two",
                        "teaser two",
                        "description two",
                        "payment details text two",
                        "reference label two",
                        "paris reference two",
                        "fund two",
                        string.Empty,
                        "icon two",
                        new List<Crumb>(),
                        EPaymentReferenceValidation.None,
                        "meta description two",
                        "return url two",
                        "catalogue id two",
                        "account reference two",
                        "payment description two",
                        new List<Alert>()
            )
        };

        _mockRepository
            .Setup(repo => repo.Get())
            .ReturnsAsync(HttpResponse.Successful(payments));

        // Act
        IActionResult result = await _controller.Index("test-business");

        // Assert
        _mockCreateRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}