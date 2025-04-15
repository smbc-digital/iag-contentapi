namespace StockportContentApiTests.Unit.Services;

public class DocumentsServiceTests
{
    private readonly DocumentsService _documentsService;
    private readonly Mock<Func<ContentfulConfig, IAssetRepository>> _mockDocumentRepositoryFunc = new();
    private readonly Mock<IAssetRepository> _mockDocumentRepository = new();
    private readonly Mock<Func<ContentfulConfig, IGroupAdvisorRepository>> _mockGroupAdvisorRepositoryFunc = new();
    private readonly Mock<IGroupAdvisorRepository> _mockGroupAdvisorRepository = new();
    private readonly Mock<Func<ContentfulConfig, IGroupRepository>> _mockGroupRepositoryFunc = new();
    private readonly Mock<IGroupRepository> _mockGroupRepository = new();
    private readonly Mock<IContentfulFactory<Asset, Document>> _mockDocumentFactory = new();
    private readonly Mock<IContentfulConfigBuilder> _mockContentfulConfigBuilder = new();
    private readonly Mock<ILoggedInHelper> _mockLoggedInHelper = new();

    public DocumentsServiceTests()
    {
        _mockDocumentRepositoryFunc
            .Setup(documentRepo => documentRepo(It.IsAny<ContentfulConfig>()))
            .Returns(_mockDocumentRepository.Object);

        _mockGroupAdvisorRepositoryFunc
            .Setup(groupAdvisorRepo => groupAdvisorRepo(It.IsAny<ContentfulConfig>()))
            .Returns(_mockGroupAdvisorRepository.Object);

        _mockGroupRepositoryFunc
            .Setup(groupRepo => groupRepo(It.IsAny<ContentfulConfig>()))
            .Returns(_mockGroupRepository.Object);

        _mockDocumentRepository
            .Setup(documentRepo => documentRepo.Get(It.IsAny<string>()))
            .ReturnsAsync(new Asset()
            {
                Description = "description",
                DescriptionLocalized = new() { { "en-GB", "description" } },
                File = new()
                {
                    Url = "url",
                    Details = new()
                    {
                        Size = 22
                    }
                },
                FilesLocalized = new() { { "en-GB", new File() } },
                SystemProperties = new()
                {
                    Id = "asset id"
                },
                Title = "title",
                TitleLocalized = new() { { "en-GB", "title" } }
            });

        _mockGroupAdvisorRepository
            .Setup(groupAdvisorRepo => groupAdvisorRepo.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(true);

        _mockGroupRepository
            .Setup(groupRepo => groupRepo.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document> { new DocumentBuilder().Build() }).Build()));

        _mockDocumentFactory
            .Setup(documentFactory => documentFactory.ToModel(It.IsAny<Asset>()))
            .Returns(new DocumentBuilder().Build());

        _mockContentfulConfigBuilder
            .Setup(config => config.Build(It.IsAny<string>()))
            .Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        _mockLoggedInHelper
            .Setup(loggedInHelper => loggedInHelper.GetLoggedInPerson())
            .Returns(new LoggedInPerson()
            {
                Email = "email",
                Name = "name"
            });

        _documentsService = new(_mockDocumentRepositoryFunc.Object,
                                                _mockGroupAdvisorRepositoryFunc.Object,
                                                _mockGroupRepositoryFunc.Object,
                                                _mockDocumentFactory.Object,
                                                _mockContentfulConfigBuilder.Object,
                                                _mockLoggedInHelper.Object);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldReturnDocument_ToAuthorisedUser()
    {
        // Arrange
        Document expectedResult = new DocumentBuilder().Build();

        // Act
        Document result = await _documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Equal("title", result.Title);
        Assert.Equal("url", result.Url);
        Assert.Equal("asset id", result.AssetId);
        Assert.Equal(22, result.Size);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_ToUnauthorisedUser()
    {
        // Arrange
        Document expectedResult = new DocumentBuilder().Build();

        _mockGroupAdvisorRepository
            .Setup(groupAdvisorRepo => groupAdvisorRepo.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(false);

        // Act
        Document result = await _documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_ToNotLoggedInUser()
    {
        // Arrange
        _mockLoggedInHelper
            .Setup(loggedInHelper => loggedInHelper.GetLoggedInPerson())
            .Returns(new LoggedInPerson());

        // Act
        Document result = await _documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfAssetDoesNotExist()
    {
        // Arrange
        Document document = new DocumentBuilder().Build();

        _mockDocumentRepository
            .Setup(documentRepo => documentRepo.Get(It.IsAny<string>()))
            .ReturnsAsync((Asset)null);

        // Act
        Document result = await _documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfGroupDoesNotReferenceAsset()
    {
        // Arrange
        Document document = new DocumentBuilder().Build();

        _mockDocumentRepository
            .Setup(documentRepo => documentRepo.Get(It.IsAny<string>()))
            .ReturnsAsync((Asset)null);

        // Act
        Document result = await _documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }
}