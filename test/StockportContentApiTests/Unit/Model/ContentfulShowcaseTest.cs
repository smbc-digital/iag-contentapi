using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
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
                FeaturedTopic = new List<Entry<ContentfulTopic>>()
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
