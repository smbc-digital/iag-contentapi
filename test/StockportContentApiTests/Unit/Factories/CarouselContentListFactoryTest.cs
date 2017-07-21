using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class CarouselContentListFactoryTest : TestingBaseClass
    {
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private CarouselContentListFactory _carouselContentListFactory;

        public CarouselContentListFactoryTest()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 09, 02));
            _carouselContentListFactory = new CarouselContentListFactory(_mockTimeProvider.Object, new CarouselContentFactory());
        }

        [Fact]
        public void ShouldBuildListOfCarouselContent()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Homepage.HomepageWithOnlyCarouselContent.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var homepage = contentfulResponse.GetFirstItem();
            IEnumerable<CarouselContent> listOfcarouselContent = _carouselContentListFactory.BuildFromReferences(homepage.fields.carouselContents, contentfulResponse);

            listOfcarouselContent.Count().Should().Be(2);
            listOfcarouselContent.First().Title.Should().Be("Watch Council meetings online");
            listOfcarouselContent.Last().Slug.Should().Be("red-rock-opening");
        }

        [Fact]
        public void ShouldReturnJustOneCarouselBasedOnTheDate()
        {
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 12));
            _carouselContentListFactory = new CarouselContentListFactory(_mockTimeProvider.Object, new CarouselContentFactory());

            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Homepage.HomepageWithOnlyCarouselContent.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var homepage = contentfulResponse.GetFirstItem();

            IEnumerable<CarouselContent> listOfcarouselContent = _carouselContentListFactory.BuildFromReferences(homepage.fields.carouselContents, contentfulResponse);

            listOfcarouselContent.Count().Should().Be(1);
        }

        [Fact]
        public void ShouldReturnEmptyListIfReferencesFieldIsEmpty()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Homepage.HomepageWithOnlyCarouselContent.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            IEnumerable<CarouselContent> listOfcarouselContent = _carouselContentListFactory.BuildFromReferences(null, contentfulResponse);

            listOfcarouselContent.Should().BeEmpty();
        }
    }
}