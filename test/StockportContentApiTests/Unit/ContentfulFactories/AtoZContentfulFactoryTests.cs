namespace StockportContentApiTests.Unit.ContentfulFactories;

public class AtoZContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateAtoZFromAContentfulReference()
    {
        // Arrange
        ContentfulAtoZ contentfulReference = new ContentfulAToZBuilder().Build();

        // Act
        AtoZ atoZ = new AtoZContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Slug, atoZ.Slug);
        Assert.Equal(contentfulReference.Title, atoZ.Title);
        Assert.Equal(contentfulReference.Sys.ContentType.SystemProperties.Id, atoZ.Type);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulAtoZ contentfulReference = new ContentfulAToZBuilder().Build();
        contentfulReference.Title = string.Empty;

        // Act
        AtoZ atoZ = new AtoZContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Name, atoZ.Title);
        Assert.Equal(contentfulReference.Sys.ContentType.SystemProperties.Id, atoZ.Type);
    }
}