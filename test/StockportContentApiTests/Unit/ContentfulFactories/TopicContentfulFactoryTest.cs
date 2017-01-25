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
    public class TopicContentfulFactoryTest
    {
        private readonly ContentfulTopic _contentfulTopic;
        private readonly Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>> _subItemFactory;
        private readonly TopicContentfulFactory _topicContentfulFactory;

        public TopicContentfulFactoryTest()
        {
            _contentfulTopic = new ContentfulTopicBuilder().Build();
            _crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            _subItemFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
            _topicContentfulFactory = new TopicContentfulFactory(_subItemFactory.Object, _crumbFactory.Object);
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopic()
        {
            var crumb = new Crumb("title", "slug", "type");
            _crumbFactory.Setup(o => o.ToModel(_contentfulTopic.Breadcrumbs.First())).Returns(crumb);
            var subItem = new SubItem("slug1", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue);
            var secondaryItem = new SubItem("slug2", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue);
            var tertiaryItem = new SubItem("slug3", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue);
            _subItemFactory.Setup(o => o.ToModel(_contentfulTopic.SubItems.First())).Returns(subItem);
            _subItemFactory.Setup(o => o.ToModel(_contentfulTopic.SecondaryItems.First())).Returns(secondaryItem);
            _subItemFactory.Setup(o => o.ToModel(_contentfulTopic.TertiaryItems.First())).Returns(tertiaryItem);

            var topic = _topicContentfulFactory.ToModel(_contentfulTopic);

            topic.ShouldBeEquivalentTo(_contentfulTopic, o => o.Excluding(e => e.Breadcrumbs)
                                                                 .Excluding(e => e.SubItems)
                                                                 .Excluding(e => e.SecondaryItems)
                                                                 .Excluding(e => e.TertiaryItems)
                                                                 .Excluding(e => e.BackgroundImage)
                                                                 .Excluding(e => e.Alerts));

            _crumbFactory.Verify(o => o.ToModel(_contentfulTopic.Breadcrumbs.First()), Times.Once);
            topic.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);
            topic.Alerts.First().ShouldBeEquivalentTo(_contentfulTopic.Alerts.First().Fields);
            _subItemFactory.Verify(o => o.ToModel(_contentfulTopic.SubItems.First()), Times.Once);
            _subItemFactory.Verify(o => o.ToModel(_contentfulTopic.SecondaryItems.First()), Times.Once);
            _subItemFactory.Verify(o => o.ToModel(_contentfulTopic.TertiaryItems.First()), Times.Once);
            topic.SubItems.First().Should().Be(subItem);
            topic.SecondaryItems.First().Should().Be(secondaryItem);
            topic.TertiaryItems.First().Should().Be(tertiaryItem);
        }

        [Fact]
        public void ShouldNotAddBreadcrumbsOrSubItemsOrSecondaryItemsOrTertiaryItemsOrImageOrAlertsIfTheyAreLinks()
        {
            _contentfulTopic.Breadcrumbs.First().SystemProperties.Type = "Link";
            _contentfulTopic.SubItems.First().SystemProperties.Type = "Link";
            _contentfulTopic.SecondaryItems.First().SystemProperties.Type = "Link";
            _contentfulTopic.TertiaryItems.First().SystemProperties.Type = "Link";
            _contentfulTopic.Alerts.First().SystemProperties.Type = "Link";
            _contentfulTopic.BackgroundImage.SystemProperties.Type = "Link";

            var topic = _topicContentfulFactory.ToModel(_contentfulTopic);

            topic.Breadcrumbs.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(_contentfulTopic.Breadcrumbs.First()), Times.Never);
            topic.SubItems.Count().Should().Be(0);
            topic.SecondaryItems.Count().Should().Be(0);
            topic.TertiaryItems.Count().Should().Be(0);
            _subItemFactory.Verify(o => o.ToModel(It.IsAny<Entry<ContentfulSubItem>>()), Times.Never);
            topic.BackgroundImage.Should().BeEmpty();
        }
    }
}
