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
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulFactories.TopicFactories;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class TopicContentfulFactoryTest
    {
        private readonly ContentfulTopic _contentfulTopic;

        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory;
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
        private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory;
        private readonly Mock<IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>> _expandingLinkBoxFactory;
        private readonly TopicContentfulFactory _topicContentfulFactory;
        private readonly Mock<IContentfulFactory<ContentfulAdvertisement, Advertisement>> _advertisementFactory;
        private readonly Mock<ITimeProvider> _timeProvider = new Mock<ITimeProvider>();

        public TopicContentfulFactoryTest()
        {
            _contentfulTopic = new ContentfulTopicBuilder().Build();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _subItemFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _eventBannerFactory = new Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>>();
            _expandingLinkBoxFactory = new Mock<IContentfulFactory<ContentfulExpandingLinkBox, ExpandingLinkBox>>();
            _advertisementFactory = new Mock<IContentfulFactory<ContentfulAdvertisement, Advertisement>>();
            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 02, 02));
            _topicContentfulFactory = new TopicContentfulFactory(_subItemFactory.Object, _crumbFactory.Object, _alertFactory.Object, _eventBannerFactory.Object, _expandingLinkBoxFactory.Object, _advertisementFactory.Object, _timeProvider.Object, HttpContextFake.GetHttpContextFake());
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopic()
        {
            //Arrange
            var crumb = new Crumb("title", "slug", "type");
            _crumbFactory.Setup(_ => _.ToModel(_contentfulTopic.Breadcrumbs.First())).Returns(crumb);

            var advertisement = new Advertisement("Advert Title", "advert slug", "advert teaser", DateTime.MaxValue.ToUniversalTime(), DateTime.MaxValue.ToUniversalTime(), true, "url", "image.jpg");
            _advertisementFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulAdvertisement>())).Returns(advertisement);

            var subItem = new SubItem("slug1", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
            _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.SubItems.First())).Returns(subItem);

            var secondaryItem = new SubItem("slug2", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
            _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.SecondaryItems.First())).Returns(secondaryItem);

            var tertiaryItem = new SubItem("slug3", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>());
            _subItemFactory.Setup(_ => _.ToModel(_contentfulTopic.TertiaryItems.First())).Returns(tertiaryItem);

            var eventBanner = new EventBanner("Title", "Teaser", "Icon", "Link");
            _eventBannerFactory.Setup(_ => _.ToModel(_contentfulTopic.EventBanner)).Returns(eventBanner);

            var alert = new Alert("title", "subheading", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10), string.Empty);
            _alertFactory.Setup(_ => _.ToModel(_contentfulTopic.Alerts.First())).Returns(alert);

            var expandingLinkBox = new ExpandingLinkBox("title", new List<SubItem>());
            _expandingLinkBoxFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulExpandingLinkBox>()))
                .Returns(expandingLinkBox);

            //Act
            var result = _topicContentfulFactory.ToModel(_contentfulTopic);

            //Assert
            result.Advertisement.Should().BeEquivalentTo(advertisement);
            result.SubItems.Count().Should().Be(1);
            result.SubItems.First().Should().BeEquivalentTo(subItem);
            result.SecondaryItems.Count().Should().Be(1);
            result.SecondaryItems.First().Should().BeEquivalentTo(secondaryItem);
            result.TertiaryItems.Count().Should().Be(1);
            result.TertiaryItems.First().Should().BeEquivalentTo(tertiaryItem);
            result.EventBanner.Should().BeEquivalentTo(eventBanner);
            result.Alerts.Count().Should().Be(1);
            result.Alerts.First().Should().BeEquivalentTo(alert);
            result.BackgroundImage.Should().BeEquivalentTo("background-image-url.jpg");
            result.Breadcrumbs.Count().Should().Be(1);
            result.Breadcrumbs.First().Should().BeEquivalentTo(crumb);
            result.EmailAlerts.Should().Be(false);
            result.EmailAlertsTopicId.Should().BeEquivalentTo("id");
            result.ExpandingLinkBoxes.Count().Should().Be(1);
            result.ExpandingLinkBoxes.First().Should().BeEquivalentTo(expandingLinkBox);
            result.ExpandingLinkTitle.Should().BeEquivalentTo("expandingLinkTitle");
            result.Icon.Should().BeEquivalentTo("icon");
            result.Image.Should().BeEquivalentTo("background-image-url.jpg");
            result.Slug.Should().BeEquivalentTo("slug");
            result.Name.Should().BeEquivalentTo("name");
            result.Summary.Should().BeEquivalentTo("summary");
            result.SunriseDate.Should().Be(DateTime.MinValue);
            result.SunsetDate.Should().Be(DateTime.MaxValue);
            result.Teaser.Should().BeEquivalentTo("teaser");
        }

        [Fact]
        public void ShouldNotAddBreadcrumbsOrSubItemsOrSecondaryItemsOrTertiaryItemsOrImageOrAlertsIfTheyAreLinks()
        {
            _contentfulTopic.Breadcrumbs.First().Sys.Type = "Link";
            _contentfulTopic.SubItems.First().Sys.Type = "Link";
            _contentfulTopic.SecondaryItems.First().Sys.Type = "Link";
            _contentfulTopic.TertiaryItems.First().Sys.Type = "Link";
            _contentfulTopic.Alerts.First().Sys.Type = "Link";
            _contentfulTopic.BackgroundImage.SystemProperties.Type = "Link";

            var topic = _topicContentfulFactory.ToModel(_contentfulTopic);

            topic.Breadcrumbs.Count().Should().Be(0);
            _crumbFactory.Verify(o => o.ToModel(_contentfulTopic.Breadcrumbs.First()), Times.Never);
            topic.SubItems.Count().Should().Be(0);
            topic.SecondaryItems.Count().Should().Be(0);
            topic.TertiaryItems.Count().Should().Be(0);
            _subItemFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
            topic.BackgroundImage.Should().BeEmpty();
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithOneOutsdieOfDates()
        {
            var alerts = new List<ContentfulAlert> {
                new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 01, 01)).Build(),
            new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 01, 04)).SunriseDate(new DateTime(2017, 01, 01)).Build()
            };

            var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

            var topic = _topicContentfulFactory.ToModel(contentfulTopic);

            topic.Alerts.Should().HaveCount(1);
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithAllInsideOfDates()
        {
            var alerts = new List<ContentfulAlert>{
                new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 01, 01)).Build(),
                new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 02, 03)).SunriseDate(new DateTime(2017, 01, 01)).Build()
            };

            var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

            var topic = _topicContentfulFactory.ToModel(contentfulTopic);

            topic.Alerts.Should().HaveCount(2);
        }

        [Fact]
        public void ShouldCreateATopicFromAContentfulTopicAndFilterAlertsWithAllOutsideOfDates()
        {
            var alerts = new List<ContentfulAlert> {
                new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 04, 10)).SunriseDate(new DateTime(2017, 03, 01)).Build(),
                new ContentfulAlertBuilder().SunsetDate(new DateTime(2017, 10, 03)).SunriseDate(new DateTime(2017, 03, 01)).Build()
            };

            var contentfulTopic = new ContentfulTopicBuilder().Alerts(alerts).Build();

            var topic = _topicContentfulFactory.ToModel(contentfulTopic);

            topic.Alerts.Should().HaveCount(0);
        }
    }
}
