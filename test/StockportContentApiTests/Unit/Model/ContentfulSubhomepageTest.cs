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
    public class ContentfulSubhomepageTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulSubhomepage();
            var expected = new ContentfulSubhomepage
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
