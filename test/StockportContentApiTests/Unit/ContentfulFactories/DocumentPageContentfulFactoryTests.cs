namespace StockportContentApi.ContentfulFactories;
public class DocumentPageContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<Asset, Document>> _mockDocumentFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _mockSubitemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new();
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();
    private readonly DocumentPageContentfulFactory _factory;

    public DocumentPageContentfulFactoryTests()
    {
        _factory = new DocumentPageContentfulFactory(
            _mockDocumentFactory.Object,
            _mockSubitemFactory.Object,
            _mockCrumbFactory.Object,
            _mockTimeProvider.Object);
    }

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
        ContentfulDocumentPage contentfulDocumentPage = new()
        {
            Title = "Title",
            Slug = "slug",
            Teaser = "teaser",
            MetaDescription = "meta description",
            AboutTheDocument = "about the document",
            Documents = new List<Asset> { new ContentfulDocumentBuilder().Build() },
            AwsDocuments = "awsDocuments",
            RequestAnAccessibleFormatContactInformation = "contact info",
            FurtherInformation = "further information",
            RelatedDocuments = new List<ContentfulReference> { },
            DatePublished = DateTime.Now,
            LastUpdated = DateTime.Now,
            Breadcrumbs = new List<ContentfulReference> { },
            Sys = new SystemProperties { UpdatedAt = DateTime.Now }
        };

        // Act
        DocumentPage result = _factory.ToModel(contentfulDocumentPage);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Title", result.Title);
        Assert.Equal("slug", result.Slug);
    }

    [Fact]
    public void ToModel_FiltersAndTransformsCorrectly()
    {
        // Arrange
        Document document = new()
        {
            Title = "Title",
            AssetId = "assetID",
            FileName = "FileName",
            LastUpdated = new DateTime(2024, 08, 01),
            MediaType = "pdf",
            Size = 12,
            Url = "url"
        };

        SubItem subItem = new()
        {
            Slug = "slug",
            ColourScheme = EColourScheme.Blue,
            Icon = "icon",
            Image = "image",
            Title = "title",
            SunriseDate = new DateTime(2024, 08, 01),
            SunsetDate = new DateTime(2101, 08, 01),
            Type = "type"
        };

        _mockDocumentFactory.Setup(_ => _.ToModel(It.IsAny<Asset>())).Returns(document);
        _mockSubitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(subItem);

        Crumb mockCrumb = new("title", "slug", "type");
        _mockCrumbFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(mockCrumb);
        _mockTimeProvider.Setup(_ => _.Now()).Returns(new DateTime(2024, 08, 01));

        ContentfulDocumentPage contentfulDocumentPage = new()
        {
            Slug = "slug",
            Title = "title",
            Documents = new List<Asset> { new ContentfulDocumentBuilder().Build() },
            RelatedDocuments = new List<ContentfulReference>
            {
                new() {
                    Slug = "slug",
                    Name = "name",
                    SunriseDate = new DateTime(2017, 08, 01),
                    SunsetDate = new DateTime(2101, 08, 01),
                    Sections = new List<ContentfulSection>(),
                    Sys = new() { UpdatedAt = new DateTime(2024, 08, 01) },
                    SubItems = new List<ContentfulReference>() { new() { Sys = new() { UpdatedAt = new DateTime(2024, 08, 01) } } }
                }
            },
            Breadcrumbs = new List<ContentfulReference> { new() { Slug = "slug", Sections = new List<ContentfulSection>() } },
            Sys = new()
            {
                UpdatedAt = new DateTime(2024, 08, 01)
            }
        };

        // Act
        DocumentPage result = _factory.ToModel(contentfulDocumentPage);

        // Assert
        _mockDocumentFactory.Verify(_ => _.ToModel(It.IsAny<Asset>()), Times.Once);
        _mockSubitemFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Once);
        _mockCrumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Once);

        Assert.NotNull(result);
        Assert.NotNull(result.Documents);
        Assert.Equal(contentfulDocumentPage.Documents.Count, result.Documents.Count);
        Assert.NotNull(result.RelatedDocuments);
        Assert.Equal(contentfulDocumentPage.RelatedDocuments.Count, result.RelatedDocuments.Count);
        Assert.NotNull(result.Breadcrumbs);
    }
}