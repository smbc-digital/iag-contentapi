namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CrumbContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateACrumbFromAContentfulReference()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder().Build();
        
        // Act
        Crumb crumb = new CrumbContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Slug, crumb.Slug);
        Assert.Equal(contentfulReference.Title, crumb.Title);
        Assert.Equal(contentfulReference.Sys.ContentType.SystemProperties.Id, crumb.Type);
    }

    [Fact]
    public void ToModel_ShouldCreateACrumbWithNameIfSet()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder().Name("name").Title(string.Empty).Build();
        
        // Act
        Crumb crumb = new CrumbContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Name, crumb.Title);
    }
}