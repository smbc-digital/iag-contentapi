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
    public class EventContentfulFactoryTest
    {
        private readonly ContentfulEvent _contentfulEvent;
        private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
        private readonly EventContentfulFactory _eventContentfulFactory;

        public EventContentfulFactoryTest()
        {
            _contentfulEvent = new ContentfulEventBuilder().Build();

            _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            _eventContentfulFactory = new EventContentfulFactory(_documentFactory.Object);
        }

        [Fact]
        public void ShouldCreateAnEventFromAContentfulEvent()
        {
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            _documentFactory.Setup(o => o.ToModel(_contentfulEvent.Documents.First())).Returns(document);

            var anEvent = _eventContentfulFactory.ToModel(_contentfulEvent);

            var mapPosition = new MapPosition() {Lat = 53.5, Lon = -2.5};

            anEvent.ShouldBeEquivalentTo(_contentfulEvent, o => o.Excluding(e => e.ImageUrl).Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Documents));
            anEvent.ImageUrl.Should().Be(_contentfulEvent.Image.File.Url);
            anEvent.ThumbnailImageUrl.Should().Be(_contentfulEvent.Image.File.Url + "?h=250");
            anEvent.Documents.Count.Should().Be(1);
            anEvent.Documents.First().Should().Be(document);
            anEvent.MapPosition.Lat.Should().Be(mapPosition.Lat);
            anEvent.MapPosition.Lon.Should().Be(mapPosition.Lon);
            anEvent.Featured.Should().BeFalse();
            _documentFactory.Verify(o => o.ToModel(_contentfulEvent.Documents.First()), Times.Once);
        }

        [Fact]
        public void ShouldNotAddDocumentsOrImageIfTheyAreLinks() 
        {
            _contentfulEvent.Documents.First().SystemProperties.Type = "Link";
            _contentfulEvent.Image.SystemProperties.Type = "Link";

            var anEvent = _eventContentfulFactory.ToModel(_contentfulEvent);

            anEvent.Documents.Count.Should().Be(0);
            anEvent.ImageUrl.Should().Be(string.Empty);
            anEvent.ThumbnailImageUrl.Should().Be(string.Empty);
        }
    }
}
