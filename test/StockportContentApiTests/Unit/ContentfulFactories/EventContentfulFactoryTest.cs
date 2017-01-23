using System;
using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class EventContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAnEventFromAContentfulEvent()
        {
            var contentfulEvent = new ContentfulEventBuilder().Build();

            var documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
            var document = new Document("title", 1000, DateTime.MinValue.ToUniversalTime(), "url", "fileName");
            documentFactory.Setup(o => o.ToModel(contentfulEvent.Documents.First())).Returns(document);

            var anEvent = new EventContentfulFactory(documentFactory.Object).ToModel(contentfulEvent);

            anEvent.ShouldBeEquivalentTo(contentfulEvent, o => o.Excluding(e => e.ImageUrl).Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Documents));
            anEvent.ImageUrl.Should().Be(contentfulEvent.Image.File.Url);
            anEvent.ThumbnailImageUrl.Should().Be(contentfulEvent.Image.File.Url + "?h=250");

            anEvent.Documents.Count.Should().Be(1);
            anEvent.Documents.First().Should().Be(document);
            documentFactory.Verify(o => o.ToModel(contentfulEvent.Documents.First()), Times.Once);
        }
    }
}
