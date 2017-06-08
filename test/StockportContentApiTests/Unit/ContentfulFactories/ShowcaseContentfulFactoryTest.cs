﻿using System;
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

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ShowcaseContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAShowcaseFromAContentfulShowcase()
        {
            var subItems = new List<SubItem> {
                new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()) };
            var crumb = new Crumb("title", "slug", "type");

            var contentfulShowcase = new ContentfulShowcaseBuilder()
                .Title("showcase title")
                .Slug("showcase-slug")
                .HeroImage(new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } })
                .Teaser("showcase teaser")
                .Subheading("subheading")
                .FeaturedItems(new List<ContentfulSubItem>() {new ContentfulSubItem() { Sys = new SystemProperties() {Type = "Entry"} } })
                .Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulSubItem, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSubItem>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<ContentfulCrumb, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulCrumb>())).Returns(crumb);

            Mock<ITimeProvider> _timeprovider = new Mock<ITimeProvider>();

            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, _timeprovider.Object);

            var showcase = contentfulFactory.ToModel(contentfulShowcase);

            showcase.Should().BeOfType<Showcase>();
            showcase.Slug.Should().Be("showcase-slug");
            showcase.Title.Should().Be("showcase title");
            showcase.HeroImageUrl.Should().Be("image-url.jpg");
            showcase.Teaser.Should().Be("showcase teaser");
            showcase.Subheading.Should().Be("subheading");
            showcase.FeaturedItems.First().Title.Should().Be(subItems.First().Title);
            showcase.FeaturedItems.First().Icon.Should().Be(subItems.First().Icon);
            showcase.FeaturedItems.First().Slug.Should().Be(subItems.First().Slug);
            showcase.FeaturedItems.Should().HaveCount(1);

        }

        [Fact]
        public void ShouldCreateAShowcaseWithAnEmptyFeaturedItems()
        {
            var contentfulShowcase = new ContentfulShowcaseBuilder().FeaturedItems(new List<ContentfulSubItem>()).Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulSubItem, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSubItem>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<ContentfulCrumb, Crumb>>();

            var _timeprovider = new Mock<ITimeProvider>();

            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, _timeprovider.Object);

            var model = contentfulFactory.ToModel(contentfulShowcase);

            model.Should().BeOfType<Showcase>();
            model.FeaturedItems.Should().BeEmpty();
        }

        [Fact]
        public void ShouldCreateAShowcaseWithoutExpiredSubItems()
        {
            var subItemThatShouldDisplay = new ContentfulSubItem()
            {

                Title = "Sub1",
                SunriseDate = new DateTime(2016, 12, 30),
                SunsetDate = new DateTime(2018, 12, 30),
                Sys = new SystemProperties() { Type = "Entry" }

            };

            var subItemThatShouldHaveExpired = new ContentfulSubItem()
            {
                Title = "Sub1",
                SunriseDate = new DateTime(2016, 12, 30),
                SunsetDate = new DateTime(2017, 01, 30),
                Sys = new SystemProperties() { Type = "Entry" }
            };

            var subItems = new List<ContentfulSubItem> {subItemThatShouldDisplay, subItemThatShouldHaveExpired};

            var contentfulShowcase = new ContentfulShowcaseBuilder().FeaturedItems(subItems).Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulSubItem, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSubItem>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<ContentfulCrumb, Crumb>>();

            var _timeprovider = new Mock<ITimeProvider>();

            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, _timeprovider.Object);

            var model = contentfulFactory.ToModel(contentfulShowcase);

            model.Should().BeOfType<Showcase>();
            model.FeaturedItems.Count().Should().Be(1);
        }

        [Fact]
        public void ShouldCreateAShowcaseWithoutSubItemsThatHaveNotArisen()
        {
            var subItemThatShouldNotYetDisplay = new ContentfulSubItem()
            {
                Title = "Sub1",
                SunriseDate = new DateTime(2018, 12, 30),
                SunsetDate = new DateTime(2019, 12, 30),
                Sys = new SystemProperties() {Type = "Entry"}
            };

            var subItemThatShouldDisplay = new ContentfulSubItem()
            {
                Title = "Sub1",
                SunriseDate = new DateTime(2016, 12, 30),
                SunsetDate = new DateTime(2018, 01, 30),
                Sys = new SystemProperties() { Type = "Entry" }
            };

            var subItems = new List<ContentfulSubItem>();
            subItems.Add(subItemThatShouldNotYetDisplay);
            subItems.Add(subItemThatShouldDisplay);

            var contentfulShowcase = new ContentfulShowcaseBuilder().FeaturedItems(subItems).Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulSubItem, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSubItem>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<ContentfulCrumb, Crumb>>();

            var _timeprovider = new Mock<ITimeProvider>();

            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, _timeprovider.Object);

            var model = contentfulFactory.ToModel(contentfulShowcase);

            model.Should().BeOfType<Showcase>();
            model.FeaturedItems.Count().Should().Be(1);
        }
    }
}
