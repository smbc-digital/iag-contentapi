namespace StockportContentApiTests.Unit.ContentfulFactories;

public class DocumentContentfulFactoryTest
{
    [Fact]
    public void ToModel_ShouldCreateADocumentFromAContentfulDocument()
    {
        // Arrange
        Asset contentfulDocument = new ContentfulDocumentBuilder().Build();

        // Act
        Document document = new DocumentContentfulFactory().ToModel(contentfulDocument);

        // Assert
        Assert.Equal(contentfulDocument.File.FileName, document.FileName);
        Assert.Equal(contentfulDocument.Description, document.Title);
        Assert.Equal(contentfulDocument.SystemProperties.UpdatedAt.Value, document.LastUpdated);
        Assert.Equal(contentfulDocument.File.Details.Size, document.Size);
        Assert.Equal(contentfulDocument.File.Url, document.Url);
    }
}