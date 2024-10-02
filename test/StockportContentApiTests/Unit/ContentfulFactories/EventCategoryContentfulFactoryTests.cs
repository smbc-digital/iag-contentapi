namespace StockportContentApiTests.Unit.ContentfulFactories;

public class EventCategoryContentfulFactoryTests
{
    [Fact]
    public void ShouldCreateAEventCategoryFromAContentfulEventCategory()
    {

        ContentfulEventCategory contentfulShowcase = new ContentfulEventCategoryBuilder()
            .Name("category name")
            .Slug("category-slug")
            .Icon("icon")
            .Build();

        EventCategoryContentfulFactory contentfulFactory = new();

        EventCategory category = contentfulFactory.ToModel(contentfulShowcase);

        category.Should().BeOfType<EventCategory>();
        category.Name.Should().Be("category name");
        category.Slug.Should().Be("category-slug");
        category.Icon.Should().Be("icon");
    }
}