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
                .FeaturedItems(new List<Entry<ContentfulSubItem>>() {new Entry<ContentfulSubItem>() {Fields = new ContentfulSubItemBuilder().Build(), SystemProperties = new SystemProperties() {Type = "Entry"} } })
                .Build();

            var topicFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulSubItem>>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>())).Returns(crumb);

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object);

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
            var contentfulShowcase = new ContentfulShowcaseBuilder().FeaturedItems(new List<Entry<ContentfulSubItem>>()).Build();

            var topicFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulSubItem>>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object);

            var model = contentfulFactory.ToModel(contentfulShowcase);

            model.Should().BeOfType<Showcase>();
            model.FeaturedItems.Should().BeEmpty();
        }
    }
}
