namespace StockportContentApiTests.Unit.Services;

public class DocumentsServiceTests
{
    private readonly DocumentsService _documentsService;
    private readonly Mock<Func<ContentfulConfig, IAssetRepository>> _mockDocumentRepositoryFunc = new();
    private readonly Mock<IAssetRepository> _mockDocumentRepository = new();
    private readonly Mock<IContentfulFactory<Asset, Document>> _mockDocumentFactory = new();
    private readonly Mock<IContentfulConfigBuilder> _mockContentfulConfigBuilder = new();

    public DocumentsServiceTests()
    {
        _mockDocumentRepositoryFunc
            .Setup(documentRepo => documentRepo(It.IsAny<ContentfulConfig>()))
            .Returns(_mockDocumentRepository.Object);

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

        _mockDocumentFactory
            .Setup(documentFactory => documentFactory.ToModel(It.IsAny<Asset>()))
            .Returns(new DocumentBuilder().Build());

        _mockContentfulConfigBuilder
            .Setup(config => config.Build(It.IsAny<string>()))
            .Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        _documentsService = new(_mockDocumentRepositoryFunc.Object,
                                _mockDocumentFactory.Object,
                                _mockContentfulConfigBuilder.Object);
    }

    [Fact]
    public async Task GetSecureAssetByDocumentId_ShouldReturnDocument_ToAuthorisedUser()
    {
        // Arrange
        Document expectedResult = new DocumentBuilder().Build();

        // Act
        Document result = await _documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id", "slug");

        // Assert
        Assert.Equal(expectedResult.Title, result.Title);
        Assert.Equal(expectedResult.Url, result.Url);
        Assert.Equal(expectedResult.AssetId, result.AssetId);
        Assert.Equal(expectedResult.Size, result.Size);
    }

    [Fact]
    public async Task GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfAssetDoesNotExist()
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
    public async Task GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfGroupDoesNotReferenceAsset()
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