using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Builders;
using System;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ExpandingLinkBoxContentfulfactoryTest
    {
        private readonly Mock<ITimeProvider> _timeProvider = new Mock<ITimeProvider>();

        public ExpandingLinkBoxContentfulfactoryTest()
        {
            _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 02, 02));
        }

        [Fact]
        public void ShouldCreateAnExpandingLinkBoxFromAContentfulExpandingLinkBox()
        {
            var contentfulExpandingLinkBox = new ContentfulExpandingLinkBoxBuilder().Build();
            var subItemFactory = new SubItemContentfulFactory(_timeProvider.Object);
            var factory = new ExpandingLinkBoxContentfulfactory(subItemFactory, _timeProvider.Object);
           
            var expandingLinkBox = factory.ToModel(contentfulExpandingLinkBox);
          
            expandingLinkBox.Title.Should().Be(contentfulExpandingLinkBox.Title);
            expandingLinkBox.Links[0].Slug.Should().Be(contentfulExpandingLinkBox.Links[0].Slug);
        }
    }
}
