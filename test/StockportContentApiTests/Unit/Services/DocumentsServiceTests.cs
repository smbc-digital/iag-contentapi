﻿namespace StockportContentApiTests.Unit.Services;

public class DocumentsServiceTests
{
    private readonly Mock<Func<ContentfulConfig, IAssetRepository>> _mockDocumentRepositoryFunc = new();
    private readonly Mock<IAssetRepository> _mockDocumentRepository = new();
    private readonly Mock<IContentfulFactory<Asset, Document>> _mockDocumentFactory = new();
    private readonly Mock<IContentfulConfigBuilder> _mockContentfulConfigBuilder = new();

    public DocumentsServiceTests() =>
        _mockDocumentRepositoryFunc
            .Setup(repo => repo(It.IsAny<ContentfulConfig>()))
            .Returns(_mockDocumentRepository.Object);

    [Fact]
    public async Task GetSecureAssetByDocumentId_ShouldReturnDocument_ToAuthorisedUser()
    {
        // Arrange
        Document expectedResult = new DocumentBuilder().Build();

        _mockDocumentRepository
            .Setup(repo => repo.Get(It.IsAny<string>()))
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
            .Setup(factory => factory.ToModel(It.IsAny<Asset>()))
            .Returns(expectedResult);

        _mockContentfulConfigBuilder.Setup(_ => _.Build(It.IsAny<string>())).Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        DocumentsService documentsService = new(_mockDocumentRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object);

        // Act
        Document result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id");

        // Assert
        Assert.Equal(expectedResult, result);
    }

    [Fact]
    public async Task GetSecureAssetByDocumentId_ShouldNotReturnDocument_IfAssetDoesNotExist()
    {
        // Arrange
        Document document = new DocumentBuilder().Build();

        _mockDocumentRepository
            .Setup(repo => repo.Get(It.IsAny<string>()))
            .ReturnsAsync((Asset)null);

        _mockDocumentFactory
            .Setup(factory => factory.ToModel(It.IsAny<Asset>()))
            .Returns(document);

        _mockContentfulConfigBuilder
            .Setup(builder => builder.Build(It.IsAny<string>()))
            .Returns(new ContentfulConfig(string.Empty, string.Empty, string.Empty));

        DocumentsService documentsService = new(_mockDocumentRepositoryFunc.Object, _mockDocumentFactory.Object, _mockContentfulConfigBuilder.Object);

        // Act
        Document result = await documentsService.GetSecureDocumentByAssetId("stockportgov", "asset id");

        // Assert
        Assert.Null(result);
    }
}