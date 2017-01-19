using System.Linq;
using FluentAssertions;
using StockportContentApi.ContentfulFactories;
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

            var anEvent = new EventContentfulFactory().ToModel(contentfulEvent);

            anEvent.ShouldBeEquivalentTo(contentfulEvent, o => o.Excluding(e => e.ImageUrl).Excluding(e => e.ThumbnailImageUrl).Excluding(e => e.Documents));
            anEvent.ImageUrl.Should().Be(contentfulEvent.Image.File.Url);
            anEvent.ThumbnailImageUrl.Should().Be(contentfulEvent.Image.File.Url + "?h=250");
            anEvent.Documents.Count.Should().Be(contentfulEvent.Documents.Count);
            anEvent.Documents.First().Url.Should().Be(contentfulEvent.Documents.First().File.Url);
            anEvent.Documents.First().Title.Should().Be(contentfulEvent.Documents.First().Description);
            anEvent.Documents.First().FileName.Should().Be(contentfulEvent.Documents.First().File.FileName);
            anEvent.Documents.First().Size.Should().Be((int)contentfulEvent.Documents.First().File.Details.Size);
            anEvent.Documents.First().LastUpdated.Should().Be(contentfulEvent.Documents.First().SystemProperties.UpdatedAt.Value);
        }
    }
}
