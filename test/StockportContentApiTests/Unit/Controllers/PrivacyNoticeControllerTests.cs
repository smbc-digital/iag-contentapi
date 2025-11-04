namespace StockportContentApiTests.Unit.Controllers;

public class PrivacyNoticeControllerTests
{
    private readonly Mock<Func<string, IPrivacyNoticeRepository>> _createRepository = new();
    private readonly Mock<IPrivacyNoticeRepository> _repository = new();
    private readonly PrivacyNoticeController _controller;

    public PrivacyNoticeControllerTests()
    {
        Mock<ILogger<ResponseHandler>> logger = new();

        _createRepository.
            Setup(createRepo => createRepo(It.IsAny<string>()))
            .Returns(_repository.Object);

        _controller = new PrivacyNoticeController(new(logger.Object), _createRepository.Object);
    }

    [Fact]
    public async Task GetPrivacyNotice_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        PrivacyNotice privacyNotice = new()
        {
            Title = "Privacy notice",
            Slug = "privacy-notice"
        };

        _repository
            .Setup(repo => repo.GetPrivacyNotice(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(privacyNotice));

        // Act
        IActionResult result = await _controller.GetPrivacyNotice("privacy-notice", "test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetAllPrivacyNotices_ReturnsOkResult_WhenRepositoryReturnsSuccessfulResponse()
    {
        // Arrange
        List<PrivacyNotice> privacyNotices = new()
        {
            new PrivacyNotice()
            {
                Title = "Privacy notice",
                Slug = "privacy-notice"
            },
            new PrivacyNotice()
            {
                Title = "About this privacy notice",
                Slug = "about-this-privacy-notice"
            },
        };

        _repository
            .Setup(repo => repo.GetAllPrivacyNotices(It.IsAny<string>()))
            .ReturnsAsync(HttpResponse.Successful(privacyNotices));

        // Act
        IActionResult result = await _controller.GetAllPrivacyNotices("test-business");

        // Assert
        _createRepository.Verify(factory => factory(It.IsAny<string>()), Times.Once);
    }
}