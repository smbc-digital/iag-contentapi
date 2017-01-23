using FluentAssertions;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulCrumbTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulCrumb();
            var expected = new ContentfulCrumb
            {
               Title = string.Empty,
               Slug = string.Empty
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
