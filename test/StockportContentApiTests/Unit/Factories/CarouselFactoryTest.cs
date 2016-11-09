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
    public class CarouselFactoryTest
    {
        private readonly CarouselContentFactory _carouselContentFactory;

        public CarouselFactoryTest()
        {
            _carouselContentFactory = new CarouselContentFactory();
        }

        [Fact]
        public void ShouldBuildCarouselContent()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/CarouselContent.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var carouselContent = (CarouselContent) _carouselContentFactory.Build(contentfulResponse.GetFirstItem(),
                contentfulResponse);

            carouselContent.Title.Should().Be("Red Rock is opening this Autumn");
            carouselContent.Slug.Should().Be("red-rock-opening");
            carouselContent.Teaser.Should().Be("The long awaited cinema complex is due to open late Oct 2016. Come and take a look.");

            DateTime sunriseDate;
            DateTime.TryParse("2016-09-01T00:00+01:00", out sunriseDate);
            carouselContent.SunriseDate.Should().Be(sunriseDate);

            DateTime sunsetDate;
            DateTime.TryParse("2016-09-29T00:00+01:00", out sunsetDate);
            carouselContent.SunsetDate.Should().Be(sunsetDate);

            carouselContent.Url.Should().Be("http://fake.url");
            carouselContent.Image.Should().Be("image.jpg");

        }
    }
}