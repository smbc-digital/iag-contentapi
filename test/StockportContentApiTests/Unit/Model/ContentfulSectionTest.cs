using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using Xunit;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulSectionTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulSection();
            var expected = new ContentfulSection
            {
                Title = string.Empty,
                Slug = string.Empty,
                Body = string.Empty,
                Profiles = new List<ContentfulProfile>(),
                Documents = new List<Asset>(),
                SunriseDate = DateTime.MinValue.ToUniversalTime(),
                SunsetDate = DateTime.MaxValue.ToUniversalTime()
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}
