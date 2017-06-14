using System;
using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class TopicContentfulFactoryTest
    {
        private readonly ContentfulTopic _contentfulTopic;
        private readonly Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>> _subItemFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulAlert>, Alert>> _alertFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulEventBanner>, EventBanner>> _eventBannerFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulExpandingLinkBox>, ExpandingLinkBox>> _expandingLinkBoxFactory;
        private readonly TopicContentfulFactory _topicContentfulFactory;
        private readonly Mock<ITimeProvider> _timeProvider = new Mock<ITimeProvider>();

        public TopicContentfulFactoryTest()
        {
            _contentfulTopic = new ContentfulTopicBuilder().Build();
            _crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            _subItemFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
            _alertFactory = new Mock<IContentfulFactory<Entry<ContentfulAlert>, Alert>>();
            _eventBannerFactory = new Mock<IContentfulFactory<Entry<ContentfulEventBanner>, EventBanner>>();
            _expandingLinkBoxFactory = new Mock<IContentfulFactory<Entry<ContentfulExpandingLinkBox>, ExpandingLinkBox>>();
            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 02, 02));
            _topicContentfulFactory = new TopicContentfulFactory(_subItemFactory.Object, _crumbFactory.Object, _alertFactory.Object, _eventBannerFactory.Object, _expandingLinkBoxFactory.Object, _timeProvider.Object);
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopic()
        {
            var crumb = new Crumb("title", "slug", "type");
            _crumbFactory.Setup(o => o.ToModel(_contentfulTopic.Breadcrumbs.First())).Returns(crumb);
            var subItem = new SubItem("slug1", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
            var secondaryItem = new SubItem("slug2", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
            var tertiaryItem = new SubItem("slug3", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
            var eventBanner = new EventBanner("Title","Teaser","Icon","Link");

            _subItemFactory.Setup(o => o.ToModel(_contentfulTopic.SubItems.First())).Returns(subItem);
            _subItemFactory.Setup(o => o.ToModel(_contentfulTopic.SecondaryItems.First())).Returns(secondaryItem);
            _subItemFactory.Setup(o => o.ToModel(_contentfulTopic.TertiaryItems.First())).Returns(tertiaryItem);
            _eventBannerFactory.Setup(o => o.ToModel(_contentfulTopic.EventBanner)).Returns(eventBanner);

            var alert = new Alert("title", "subheading", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10));
            _alertFactory.Setup(o => o.ToModel(_contentfulTopic.Alerts.First())).Returns(alert);

            var topic = _topicContentfulFactory.ToModel(_contentfulTopic);

            topic.ShouldBeEquivalentTo(_contentfulTopic, o => o.Excluding(e => e.Breadcrumbs)
                                                                 .Excluding(e => e.SubItems)
                                                                 .Excluding(e => e.SecondaryItems)
                                                                 .Excluding(e => e.TertiaryItems)
                                                                 .Excluding(e => e.BackgroundImage)
                                                                 .Excluding(e => e.Image)
                                                                 .Excluding(e => e.Alerts) 
                                                                 .Excluding(e => e.EventBanner)
                                                                 .Excluding(e => e.ExpandingLinkBoxes)                                                              
                                                                 );

            _crumbFactory.Verify(o => o.ToModel(_contentfulTopic.Breadcrumbs.First()), Times.Once);
            topic.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);
            _alertFactory.Verify(o => o.ToModel(_contentfulTopic.Alerts.First()), Times.Once);
            topic.Alerts.First().ShouldBeEquivalentTo(alert);
            _subItemFactory.Verify(o => o.ToModel(_contentfulTopic.SubItems.First()), Times.Once);
            _subItemFactory.Verify(o => o.ToModel(_contentfulTopic.SecondaryItems.First()), Times.Once);
            _subItemFactory.Verify(o => o.ToModel(_contentfulTopic.TertiaryItems.First()), Times.Once);
            topic.SubItems.First().Should().Be(subItem);
            topic.SecondaryItems.First().Should().Be(secondaryItem);
            topic.TertiaryItems.First().Should().Be(tertiaryItem);
            topic.EventBanner.Title.Should().Be(eventBanner.Title);
            topic.EventBanner.Teaser.Should().Be(eventBanner.Teaser);
            topic.EventBanner.Icon.Should().Be(eventBanner.Icon);
            topic.EventBanner.Link.Should().Be(eventBanner.Link);
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

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithOneOutsdieOfDates()
        {
            var alerts = new List<Entry<ContentfulAlert>> {
                new ContentfulEntryBuilder<ContentfulAlert>().Fields(new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 01, 01)).Build()).Build(),
                new ContentfulEntryBuilder<ContentfulAlert>().Fields(new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 01, 04)).SunriseDate(new DateTime(2017, 01, 01)).Build()).Build()
            };

            var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

            var topic = _topicContentfulFactory.ToModel(contentfulTopic);

            topic.Alerts.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithAllInsideOfDates()
        {
            var alerts = new List<Entry<ContentfulAlert>> {
                new ContentfulEntryBuilder<ContentfulAlert>().Fields(new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 01, 01)).Build()).Build(),
                new ContentfulEntryBuilder<ContentfulAlert>().Fields(new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 02, 03)).SunriseDate(new DateTime(2017, 01, 01)).Build()).Build()
            };

            var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

            var topic = _topicContentfulFactory.ToModel(contentfulTopic);

            topic.Alerts.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithAllOutsideOfDates()
        {
            var alerts = new List<Entry<ContentfulAlert>> {
                new ContentfulEntryBuilder<ContentfulAlert>().Fields(new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 03, 01)).Build()).Build(),
                new ContentfulEntryBuilder<ContentfulAlert>().Fields(new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 10, 03)).SunriseDate(new DateTime(2017, 03, 01)).Build()).Build()
            };

            var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

            var topic = _topicContentfulFactory.ToModel(contentfulTopic);

            topic.Alerts.Should().HaveCount(0);
        }
    }
}
