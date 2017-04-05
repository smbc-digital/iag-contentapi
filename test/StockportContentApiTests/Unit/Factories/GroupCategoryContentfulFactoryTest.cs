using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.ContentfulFactories;
using Xunit;
using StockportContentApiTests.Unit.Builders;
using StockportContentApiTests.Builders;

namespace StockportContentApiTests.Unit.Factories
{
    public class GroupCategoryContentfulFactoryTest
    {
        private readonly GroupCategoryContentfulFactory _groupCategoryContentfulFactory;


        public GroupCategoryContentfulFactoryTest()
        {
            _groupCategoryContentfulFactory = new GroupCategoryContentfulFactory();
        }

        [Fact]
        public void ShouldReturngroup()
        {
            const string slug = "group_slug";
            var contentfulGroupcategory = new ContentfulGroupCategoryBuilder().Slug(slug).Build();
            var groupCategory = _groupCategoryContentfulFactory.ToModel(contentfulGroupcategory);
            groupCategory.Name.Should().Be(contentfulGroupcategory.Name);
            groupCategory.Slug.Should().Be(contentfulGroupcategory.Slug);
            groupCategory.ImageUrl.Should().Be(contentfulGroupcategory.Image.File.Url);
            groupCategory.Icon.Should().Be(contentfulGroupcategory.Icon);
        }
    }
}
