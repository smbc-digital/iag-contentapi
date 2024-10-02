namespace StockportContentApiTests.Unit.ContentfulFactories;

public class GroupCategoryContentfulFactoryTests
{
    [Fact]
    public void ShouldCreateAGroupCategoryFromAContentfulGroupCategory()
    {
        ContentfulGroupCategory contentfulShowcase = new ContentfulGroupCategoryBuilder()
            .Name("category name")
            .Slug("category-slug")
            .Image(new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } })
            .Icon("icon")
            .Build();

        GroupCategoryContentfulFactory contentfulFactory = new();

        GroupCategory category = contentfulFactory.ToModel(contentfulShowcase);

        category.Should().BeOfType<GroupCategory>();
        category.Name.Should().Be("category name");
        category.Slug.Should().Be("category-slug");
        category.Icon.Should().Be("icon");
        category.ImageUrl.Should().Be("image-url.jpg");
    }
}