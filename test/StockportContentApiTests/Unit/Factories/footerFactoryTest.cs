using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class FooterFactoryTest
    {
        private readonly IFactory<Footer> _factory;

        private readonly Mock<IBuildContentTypesFromReferences<SocialMediaLink>> _mockSocialMediaLinkListFactory;
        private readonly Mock<IBuildContentTypesFromReferences<SubItem>> _mockSubItemListFactory;

        public FooterFactoryTest()
        {
            _mockSocialMediaLinkListFactory = new Mock<IBuildContentTypesFromReferences<SocialMediaLink>>();
            _mockSocialMediaLinkListFactory.Setup(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<SocialMediaLink> { new SocialMediaLink("slug", "title", "url", "icon") });

            _mockSubItemListFactory = new Mock<IBuildContentTypesFromReferences<SubItem>>();
            _mockSubItemListFactory.Setup(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<SubItem> { new SubItem("slug", "title", "teaser", "ison", string.Empty, DateTime.MinValue, DateTime.MinValue, "image", new List<SubItem>()) });


            _factory = new FooterFactory(_mockSubItemListFactory.Object, _mockSocialMediaLinkListFactory.Object);
        }

        [Fact]
        public void ItGetsTheFooter()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Footer.json"));   
            var contentfulResponse = new ContentfulResponse(mockContentfulData);
            
            var footer = (Footer) _factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            footer.Title.Should().Be("Footer");
            footer.Slug.Should().Be("a-slug");
            footer.Copyright.Should().Be("© 2016 A Council Name");
        }

        [Fact]
        public void BuildsFooterThatIncludesSubItems()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Footer.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var footer = (Footer)_factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            _mockSubItemListFactory.Verify(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()), Times.Once);
        }

        [Fact]
        public void BuildsFooterThatIncludesSocialMedia()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Footer.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var footer = (Footer)_factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            _mockSocialMediaLinkListFactory.Verify(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()), Times.Once);
        }
    }
}
