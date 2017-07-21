using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class AtoZFactoryTest : TestingBaseClass
    {
        private readonly IFactory<AtoZ> _factory;

        public AtoZFactoryTest()
        {
            _factory = new AtoZFactory();
        }

        [Fact]
        public void BuildAtoZFromAnArticle()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.AtoZ.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            AtoZ atoz = _factory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            atoz.Title.Should().Be("Vintage Village turns 6 years old");
            atoz.Slug.Should().Be("vintage-village-turns-6-years-old");
            atoz.Teaser.Should().Be("The vintage village turned 6 with a great reception");
            atoz.Type.Should().Be("article");
            atoz.AlternativeTitles.Count.Should().Be(2);
            atoz.AlternativeTitles[0].Should().Be("The First Article");
            atoz.AlternativeTitles[1].Should().Be("Article 1: Awesome");
        }

        [Fact]
        public void BuildAtoZFromATopic()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.AtoZTopic.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            AtoZ atoz = _factory.Build(contentfulResponse.Items.FirstOrDefault(), contentfulResponse);

            atoz.Title.Should().Be("D Letter Topic");
            atoz.Slug.Should().Be("d-letter-topic");
            atoz.Teaser.Should().Be("This is a d letter topic");
            atoz.Type.Should().Be("topic");
            atoz.AlternativeTitles.Count.Should().Be(1);
            atoz.AlternativeTitles[0].Should().Be("C Letter Topic");
        }

        [Fact]
        public void ShouldSetAlternativeTitlesToAnEmptyListIfNonProvided()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.AtoZTopic.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            AtoZ atoz = _factory.Build(contentfulResponse.Items[1], contentfulResponse);

            atoz.AlternativeTitles.Count.Should().Be(0);
        }
    }
}
