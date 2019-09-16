using System;
using System.Collections.Generic;
using System.Linq;
using Amazon.Runtime.Internal;
using Contentful.Core.Models;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class CommsContentfulFactoryTest
    {
        private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
        private readonly Mock<IContentfulFactory<IEnumerable<ContentfulBasicLink>, IEnumerable<BasicLink>>> _basicLinkFactory = new Mock<IContentfulFactory<IEnumerable<ContentfulBasicLink>, IEnumerable<BasicLink>>>();
        private readonly CommsContentfulFactory _factory;

        public CommsContentfulFactoryTest()
        {
            _factory = new CommsContentfulFactory(
                _callToActionFactory.Object,
                _eventFactory.Object,
                _basicLinkFactory.Object
            );
        }

        [Fact]
        public void ToModel_ShouldReturnLinkList()
        {
            // Arrange
            _callToActionFactory
                .Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>()))
                .Returns(new CallToActionBanner());
            _eventFactory
                .Setup(_ => _.ToModel(It.IsAny<ContentfulEvent>()))
                .Returns(new EventBuilder().Build());
            _basicLinkFactory
                .Setup(_ => _.ToModel(It.IsAny<IEnumerable<ContentfulBasicLink>>()))
                .Returns(new List<BasicLink>());

            var model = new ContentfulCommsHomepage
            {
                WhatsOnInStockportEvent = new ContentfulEventBuilder().Build(),
                MetaDescription = "meta description",
                CallToActionBanner = new ContentfulCallToActionBanner(),
                UsefullLinks = new AutoConstructedList<ContentfulBasicLink>(),
                TwitterFeedHeader = "twiiter",
                InstagramFeedTitle = "instagram header",
                FacebookFeedTitle = "facebook",
                Title = "title",
                InstagramLink = "instagram link",
                LatestNewsHeader = "latest news",
                Sys = new SystemProperties()
            };

            // Act
            var result = _factory.ToModel(model);

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.WhatsOnInStockportEvent);
            Assert.NotNull(result.CallToActionBanner);
            Assert.NotNull(result.UsefullLinks);
            Assert.Equal("twiiter", result.TwitterFeedHeader);
            Assert.Equal("meta description", result.MetaDescription);
            Assert.Equal("instagram header", result.InstagramFeedTitle);
            Assert.Equal("facebook", result.FacebookFeedTitle);
            Assert.Equal("title", result.Title);
            Assert.Equal("instagram link", result.InstagramLink);
            Assert.Equal("latest news", result.LatestNewsHeader);
        }
    }
}
