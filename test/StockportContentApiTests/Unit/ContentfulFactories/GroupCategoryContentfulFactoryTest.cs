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
using StockportContentApi.ContentfulFactories.GroupFactories;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class GroupCategoryContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAGroupCategoryFromAContentfulGroupCategory()
        {

            var contentfulShowcase = new ContentfulGroupCategoryBuilder()
                .Name("category name")
                .Slug("category-slug")
                .Image(new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } })
                .Icon("icon")
                .Build();

            var contentfulFactory = new GroupCategoryContentfulFactory(HttpContextFake.GetHttpContextFake());

            var category = contentfulFactory.ToModel(contentfulShowcase);

            category.Should().BeOfType<GroupCategory>();
            category.Name.Should().Be("category name");
            category.Slug.Should().Be("category-slug");
            category.Icon.Should().Be("icon");
            category.ImageUrl.Should().Be("image-url.jpg");

        }
    }
}
