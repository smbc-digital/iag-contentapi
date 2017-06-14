using System;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulReferenceTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulReference();
            var expected = new ContentfulReference
            {
               Slug = string.Empty,
               Title = string.Empty,
               Name = string.Empty,
               Teaser = string.Empty,
               Icon = string.Empty,
               SunriseDate = DateTime.MinValue.ToUniversalTime(),
               SunsetDate = DateTime.MaxValue.ToUniversalTime()
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
