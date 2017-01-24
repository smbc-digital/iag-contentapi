﻿using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using Xunit;
using FluentAssertions;

namespace StockportContentApiTests.Unit.Model
{
    public class ContentfulProfileTest
    {
        [Fact]
        public void ShouldSetDefaultsOnModel()
        {
            var actual = new ContentfulProfile();
            var expected = new ContentfulProfile
            {
                Type = string.Empty,
                Title = string.Empty,
                Slug = string.Empty,
                Subtitle = string.Empty,
                Teaser = string.Empty,
                Image = new Asset { File = new File { Url = string.Empty }},
                Body = string.Empty,
                Icon = string.Empty,
                BackgroundImage = new Asset { File = new File { Url = string.Empty }},
                Breadcrumbs = new List<Entry<ContentfulCrumb>>()
            };
            actual.ShouldBeEquivalentTo(expected);
        }
    }
}