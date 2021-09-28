using Xunit;
using FluentAssertions;
using Contentful.Core.Models;
using StockportContentApi.Model;
using StockportContentApiTests.Builders;
using StockportContentApi.ContentfulFactories.GroupFactories;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupCategoryContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAGroupCategoryFromAContentfulGroupCategory()
        {

            var contentfulShowcase = new ContentfulGroupCategoryBuilder()
                .Name("category name")
                .Slug("category-slug")
                .Image(new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } })
                .Icon("icon")
                .Build();

            var contentfulFactory = new GroupCategoryContentfulFactory();

            var category = contentfulFactory.ToModel(contentfulShowcase);

            category.Should().BeOfType<GroupCategory>();
            category.Name.Should().Be("category name");
            category.Slug.Should().Be("category-slug");
            category.Icon.Should().Be("icon");
            category.ImageUrl.Should().Be("image-url.jpg");

        }
    }
}
