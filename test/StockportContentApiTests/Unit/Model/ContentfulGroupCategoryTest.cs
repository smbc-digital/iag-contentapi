using Contentful.Core.Models;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulGroupCategoryTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulGroupCategory();
            var expected = new ContentfulGroupCategory
            {
                Name = string.Empty,
                Slug = string.Empty,
                Icon = string.Empty,
                Image = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } }
            };
            actual.Should().BeEquivalentTo(expected);
        }
    }
}
