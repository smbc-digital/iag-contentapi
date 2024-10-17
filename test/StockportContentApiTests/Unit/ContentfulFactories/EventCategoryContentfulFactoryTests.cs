namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventCategoryContentfulFactoryTests
{
    private readonly EventCategoryContentfulFactory _factory;

    public EventCategoryContentfulFactoryTests() => _factory = new();

    [Fact]
    public void ShouldCreateAEventCategoryFromAContentfulEventCategory()
    {
        // Arrange
        ContentfulEventCategory contentfulShowcase = new ContentfulEventCategoryBuilder()
            .Name("category name")
            .Slug("category-slug")
            .Icon("icon")
            .Build();

        // Act
        EventCategory category = _factory.ToModel(contentfulShowcase);

        // Assert
        Assert.IsType<EventCategory>(category);
        Assert.Equal("category name", category.Name);
        Assert.Equal("category-slug", category.Slug);
        Assert.Equal("icon", category.Icon);

    }
}