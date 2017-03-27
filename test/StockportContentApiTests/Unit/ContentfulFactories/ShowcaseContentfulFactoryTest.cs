using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using StockportContentApiTests.Unit.Repositories;
using Moq;
using StockportContentApi.Model;
using StockportContentApiTests.Builders;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ShowcaseContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAShowcaseFromAContentfulShowcase()
        {
            var subItems = new List<SubItem> {
                new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image") };
            var crumb = new Crumb("title", "slug", "type");
            var topic = new Topic("slug", "name", "teaser", "summary", "icon", "image", "image", subItems, subItems, subItems,
                new List<Crumb> { crumb },
                new List<Alert> { new Alert("title", "subHeading", "body", "severity", DateTime.MinValue, DateTime.MaxValue) },
                DateTime.MinValue, DateTime.MaxValue, false, "id");

            var contentfulShowcase = new ContentfulShowcaseBuilder()
                .Title("showcase title")
                .Slug("showcase-slug")
                .HeroImage(new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } })
                .Teaser("showcase teaser")
                .Subheading("subheading")
                .FeaturedItems(new List<Entry<ContentfulTopic>>() {new Entry<ContentfulTopic>() {Fields = new ContentfulTopic() {Slug = "slug"}, SystemProperties = new SystemProperties() {Type = "Entry"} } })
                .Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulTopic, Topic>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulTopic>()))
                .Returns(topic);

            var crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>())).Returns(crumb);

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object);

            var showcase = contentfulFactory.ToModel(contentfulShowcase);

            showcase.Slug.Should().Be("showcase-slug");
            showcase.Title.Should().Be("showcase title");
            showcase.HeroImageUrl.Should().Be("image-url.jpg");
            showcase.Teaser.Should().Be("showcase teaser");
            showcase.Subheading.Should().Be("subheading");
            showcase.FeaturedItems.Should().BeEquivalentTo(topic);

        }
    }
}
