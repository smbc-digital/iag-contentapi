namespace StockportContentApi.ContentfulFactories;
public class DocumentPageContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly DocumentPageContentfulFactory _factory;

    public DocumentPageContentfulFactoryTests() =>
        _factory = new DocumentPageContentfulFactory(_documentFactory.Object,
                                                    _subItemFactory.Object,
                                                    _crumbFactory.Object,
                                                    _timeProvider.Object);

    [Fact]
    public void ToModel_ReturnsNull_WhenEntryIsNull()
    {
        // Act & Assert
        Assert.Null(_factory.ToModel(null));
    }

    [Fact]
    public void ToModel_ReturnsDocumentPage_WhenEntryIsValid()
    {
        // Arrange
        ContentfulDocumentPage contentfulDocumentPage = new ContentfulDocumentPageBuilder().Build();

        // Act
        DocumentPage result = _factory.ToModel(contentfulDocumentPage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("title", result.Title);
        Assert.Equal("slug", result.Slug);
    }

    [Fact]
    public void ToModel_FiltersAndTransformsCorrectly()
    {
        // Arrange
        Document document= new DocumentBuilder().Build();
        SubItem subItem = new SubItemBuilder().Build();

        _documentFactory
            .Setup(documentFactory => documentFactory.ToModel(It.IsAny<Asset>()))
            .Returns(document);

        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(subItem);

        _crumbFactory
            .Setup(crumbFactory => crumbFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new Crumb("title", "slug", "type"));

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2024, 08, 01));

        ContentfulDocumentPage contentfulDocumentPage = new ContentfulDocumentPageBuilder().Build();

        // Act
        DocumentPage result = _factory.ToModel(contentfulDocumentPage);

        // Assert
        _documentFactory.Verify(documentFactory => documentFactory.ToModel(It.IsAny<Asset>()), Times.Once);
        _subItemFactory.Verify(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Once);
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Once);

        Assert.NotNull(result);
        Assert.NotNull(result.Documents);
        Assert.Equal(contentfulDocumentPage.Documents.Count, result.Documents.Count);
        Assert.NotNull(result.RelatedDocuments);
        Assert.Equal(contentfulDocumentPage.RelatedDocuments.Count, result.RelatedDocuments.Count);
        Assert.NotNull(result.Breadcrumbs);
    }
}