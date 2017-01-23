using System;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulSubItemTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulSubItem();
            var expected = new ContentfulSubItem
            {
               Slug = string.Empty,
               Title = string.Empty,
               Teaser = string.Empty,
               Icon = string.Empty,
               SunriseDate = DateTime.MinValue.ToUniversalTime(),
               SunsetDate = DateTime.MaxValue.ToUniversalTime()
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
