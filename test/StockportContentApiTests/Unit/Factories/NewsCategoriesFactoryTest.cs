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
    public class NewsCategoriesFactoryTest
    {
        private readonly NewsCategoriesFactory _newsCategoriesFactory;

        public NewsCategoriesFactoryTest()
        {
            _newsCategoriesFactory = new NewsCategoriesFactory();
        }

        [Fact]
        public void ShouldGenerateListOfCategoriesFromContentfulApiCAll()
        {
            var contentfulResponse = CreateResponse("Unit/MockContentfulResponses/ContentTypes.json");

            List<string> newsCategories = _newsCategoriesFactory.Build(contentfulResponse.Items);

            newsCategories.Count().Should().Be(18);
            newsCategories.First().Should().Be("Benefits");
            newsCategories.Last().Should().Be("Waste and recycling");
        }

        [Fact]
        public void ShouldGenerateEmptyListIfNewsHasNoCategories()
        {
            var contentfulResponse = CreateResponse("Unit/MockContentfulResponses/ContentTypesWithNoNewsCategories.json");

            List<string> newsCategories = _newsCategoriesFactory.Build(contentfulResponse.Items);

            newsCategories.Count().Should().Be(0);
        }

        private static ContentfulResponse CreateResponse(string stubbedContentfulJsonFile)
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText(stubbedContentfulJsonFile));
            ContentfulResponse contentfulResponse = new ContentfulResponse(mockContentfulData);
            return contentfulResponse;
        }

    }
}
