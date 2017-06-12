using System.Collections.Generic;
using Contentful.Core.Models;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulShowcaseTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulShowcase();
            var expected = new ContentfulShowcase
            {
                Title = string.Empty,
                Slug = string.Empty,
                Teaser = string.Empty,
                HeroImage = new Asset { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } },
                Subheading = string.Empty,
                FeaturedItems = new List<IContentfulSubItem>(),
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
