namespace StockportContentApiTests.Unit.Services;

public class DocumentsServiceTests
{
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
        _mockDocumentRepositoryFunc.Setup(_ => _(It.IsAny<ContentfulConfig>()))
            .Returns(_mockDocumentRepository.Object);

        _mockGroupAdvisorRepositoryFunc.Setup(_ =>
            _(It.IsAny<ContentfulConfig>())).Returns(_mockGroupAdvisorRepository.Object);

        _mockGroupRepositoryFunc
            .Setup(_ => _(It.IsAny<ContentfulConfig>())).Returns(_mockGroupRepository.Object);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldReturnDocument_ToAuthorisedUser()
    {
        // Arrange
        Document expectedResult = new DocumentBuilder().Build();

        _mockDocumentRepository.Setup(_ => _.Get(It.IsAny<string>()))
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

        _mockGroupAdvisorRepository.Setup(_ => _.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _mockGroupRepository.Setup(_ => _.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document> { expectedResult }).Build()));

        _mockDocumentFactory.Setup(_ => _.ToModel(It.IsAny<Asset>())).Returns(expectedResult);

        _mockContentfulConfigBuilder.Setup(_ => _.Build(It.IsAny<string>())).Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        _mockLoggedInHelper.Setup(_ => _.GetLoggedInPerson()).Returns(new LoggedInPerson()
        {
            Email = "email",
            Name = "name"
        });

        DocumentsService documentsService = new(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);

        // Act
        Document result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_ToUnauthorisedUser()
    {
        // Arrange
        Document expectedResult = new DocumentBuilder().Build();

        _mockDocumentRepository.Setup(_ => _.Get(It.IsAny<string>()))
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

        _mockGroupAdvisorRepository.Setup(_ => _.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

        _mockGroupRepository.Setup(_ => _.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document> { expectedResult }).Build()));

        _mockDocumentFactory.Setup(_ => _.ToModel(It.IsAny<Asset>())).Returns(expectedResult);

        _mockContentfulConfigBuilder.Setup(_ => _.Build(It.IsAny<string>())).Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        _mockLoggedInHelper.Setup(_ => _.GetLoggedInPerson()).Returns(new LoggedInPerson()
        {
            Email = "email",
            Name = "name"
        });

        DocumentsService documentsService = new(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);


        // Act
        Document result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_ToNotLoggedInUser()
    {
        // Arrange
        _mockContentfulConfigBuilder.Setup(_ => _.Build(It.IsAny<string>())).Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));
        _mockLoggedInHelper.Setup(_ => _.GetLoggedInPerson()).Returns(new LoggedInPerson());

        DocumentsService documentsService = new(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);

        // Act
        Document result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfAssetDoesNotExist()
    {
        // Arrange
        Document document = new DocumentBuilder().Build();

        _mockDocumentRepository.Setup(_ => _.Get(It.IsAny<string>()))
            .ReturnsAsync((Asset)null);

        _mockGroupAdvisorRepository.Setup(_ => _.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _mockGroupRepository.Setup(_ => _.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document> { document }).Build()));

        _mockDocumentFactory.Setup(_ => _.ToModel(It.IsAny<Asset>())).Returns(document);

        _mockContentfulConfigBuilder.Setup(_ => _.Build(It.IsAny<string>())).Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        _mockLoggedInHelper.Setup(_ => _.GetLoggedInPerson()).Returns(new LoggedInPerson()
        {
            Email = "email",
            Name = "name"
        });

        DocumentsService documentsService = new(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);

        // Act
        Document result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async void GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfGroupDoesNotReferenceAsset()
    {
        // Arrange
        Document document = new DocumentBuilder().Build();

        _mockDocumentRepository.Setup(_ => _.Get(It.IsAny<string>()))
            .ReturnsAsync((Asset)null);

        _mockGroupAdvisorRepository.Setup(_ => _.CheckIfUserHasAccessToGroupBySlug(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

        _mockGroupRepository.Setup(_ => _.GetGroup(It.IsAny<string>(), It.IsAny<bool>()))
            .ReturnsAsync(HttpResponse.Successful(new GroupBuilder().AdditionalDocuments(new List<Document>()).Build()));

        _mockDocumentFactory.Setup(_ => _.ToModel(It.IsAny<Asset>())).Returns(document);

        _mockContentfulConfigBuilder.Setup(_ => _.Build(It.IsAny<string>())).Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        _mockLoggedInHelper.Setup(_ => _.GetLoggedInPerson()).Returns(new LoggedInPerson()
        {
            Email = "email",
            Name = "name"
        });

        DocumentsService documentsService = new(_mockDocumentRepositoryFunc.Object, _mockGroupAdvisorRepositoryFunc.Object, _mockGroupRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object, _mockLoggedInHelper.Object);

        // Act
        Document result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Null(result);
    }
}