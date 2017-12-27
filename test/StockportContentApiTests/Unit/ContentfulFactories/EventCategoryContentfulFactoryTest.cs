using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using Moq;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using StockportContentApiTests.Unit.Builders;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulFactories.EventFactories;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class EventCategoryContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAEventCategoryFromAContentfulEventCategory()
        {

            var contentfulShowcase = new ContentfulEventCategoryBuilder()
                .Name("category name")
                .Slug("category-slug")
                .Icon("icon")
                .Build();

            var contentfulFactory = new EventCategoryContentfulFactory(HttpContextFake.GetHttpContextFake());

            var category = contentfulFactory.ToModel(contentfulShowcase);

            category.Should().BeOfType<EventCategory>();
            category.Name.Should().Be("category name");
            category.Slug.Should().Be("category-slug");
            category.Icon.Should().Be("icon");
        }
    }
}
