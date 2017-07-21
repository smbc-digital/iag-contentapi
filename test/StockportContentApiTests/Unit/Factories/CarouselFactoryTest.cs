using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class CarouselFactoryTest : TestingBaseClass
    {
        private readonly CarouselContentFactory _carouselContentFactory;

        public CarouselFactoryTest()
        {
            _carouselContentFactory = new CarouselContentFactory();
        }

        [Fact]
        public void ShouldBuildCarouselContent()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.CarouselContent.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var carouselContent = (CarouselContent) _carouselContentFactory.Build(contentfulResponse.GetFirstItem(),
                contentfulResponse);

            carouselContent.Title.Should().Be("Red Rock is opening this Autumn");
            carouselContent.Slug.Should().Be("red-rock-opening");
            carouselContent.Teaser.Should().Be("The long awaited cinema complex is due to open late Oct 2016. Come and take a look.");

            var sunriseDate = new DateTime(2016, 08, 31, 23, 0, 0, 0, DateTimeKind.Utc);
            carouselContent.SunriseDate.Should().Be(sunriseDate);

            var sunsetDate = new DateTime(2016, 09, 28, 23, 0, 0, 0, DateTimeKind.Utc);
            carouselContent.SunsetDate.Should().Be(sunsetDate);

            carouselContent.Url.Should().Be("http://fake.url");
            carouselContent.Image.Should().Be("image.jpg");

        }
    }
}