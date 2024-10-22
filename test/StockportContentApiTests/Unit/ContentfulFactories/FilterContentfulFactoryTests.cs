namespace StockportContentApiTests.Unit.ContentfulFactories;

public class FilterContentfulFactoryTests
{
    [Fact]
    public void ToModel_ShouldCreateFilterFromAContentfulReference()
    {
        // Arrange
        ContentfulFilter contentfulReference = new ContentfulFilterBuilder().Build();

        // Act
        Filter result = new FilterContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Slug, result.Slug);
        Assert.Equal(contentfulReference.Title, result.Title);
        Assert.Equal(contentfulReference.DisplayName, result.DisplayName);
        Assert.Equal(contentfulReference.Theme, result.Theme);
        Assert.Equal(contentfulReference.Highlight, result.Highlight);
    }

    [Fact]
    public void ToModel_ShouldMapEmptyValues()
    {
        // Arrange
        ContentfulFilter contentfulReference = new ContentfulFilterBuilder()
            .WithSlug(string.Empty)
            .WithTitle(string.Empty)
            .WithDisplayName(string.Empty)
            .WithTheme(string.Empty)
            .WithHighlight(false)
            .Build();

        // Act
        Filter result = new FilterContentfulFactory().ToModel(contentfulReference);

        // Assert
        Assert.Empty(result.Slug);
        Assert.Empty(result.Title);
        Assert.Empty(result.DisplayName);
        Assert.Empty(result.Theme);
        Assert.False(result.Highlight);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_If_EntryIsNull()
    {
        // Act
        Filter result = new FilterContentfulFactory().ToModel(null);

        // Assert
        Assert.Null(result);
    }
}