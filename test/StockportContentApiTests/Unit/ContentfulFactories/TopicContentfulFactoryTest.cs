using System;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class TopicnContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateATopicFromAContentfulTopic()
        {
            var contentfulTopic = new ContentfulTopicBuilder().Build();
            var crumb = new Crumb("title", "slug", "type");
            var crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(contentfulTopic.Breadcrumbs.First())).Returns(crumb);
            var subItem = new SubItem("slug1", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue);
            var secondaryItem = new SubItem("slug2", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue);
            var tertiaryItem = new SubItem("slug3", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue);
            var subItemFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
            subItemFactory.Setup(o => o.ToModel(contentfulTopic.SubItems.First())).Returns(subItem);
            subItemFactory.Setup(o => o.ToModel(contentfulTopic.SecondaryItems.First())).Returns(secondaryItem);
            subItemFactory.Setup(o => o.ToModel(contentfulTopic.TertiaryItems.First())).Returns(tertiaryItem);

            var section = new TopicContentfulFactory(subItemFactory.Object, crumbFactory.Object).ToModel(contentfulTopic);

            section.ShouldBeEquivalentTo(contentfulTopic, o => o.Excluding(e => e.Breadcrumbs)
                                                                .Excluding(e => e.SubItems)
                                                                .Excluding(e => e.SecondaryItems)
                                                                .Excluding(e => e.TertiaryItems)
                                                                .Excluding(e => e.BackgroundImage));

            crumbFactory.Verify(o => o.ToModel(contentfulTopic.Breadcrumbs.First()), Times.Once);
            section.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);
            section.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);

            subItemFactory.Verify(o => o.ToModel(contentfulTopic.SubItems.First()), Times.Once);
            subItemFactory.Verify(o => o.ToModel(contentfulTopic.SecondaryItems.First()), Times.Once);
            subItemFactory.Verify(o => o.ToModel(contentfulTopic.TertiaryItems.First()), Times.Once);
            section.SubItems.First().Should().Be(subItem);
            section.SecondaryItems.First().Should().Be(secondaryItem);
            section.TertiaryItems.First().Should().Be(tertiaryItem);
        }
    }
}
