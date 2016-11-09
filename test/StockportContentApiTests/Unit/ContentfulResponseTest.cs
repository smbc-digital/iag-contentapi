using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using Xunit;

namespace StockportContentApiTests.Unit
{
    public class ContentfulResponseTest
    {
        [Fact]
        public void CreatesContentfulResponseWhenAllFieldsArePresent()
        {
            var contentfulResponseDataBasicStructure =
                "{\r\n  \"items\": [\r\n  {}\r\n  ],\r\n  \"includes\": {\r\n    \"Asset\": [\r\n  {}\r\n ],\r\n    \"Entry\":[\r\n  {}\r\n ]\r\n  }\r\n}";
            var deserialisedContentfulData = JsonConvert.DeserializeObject<dynamic>(contentfulResponseDataBasicStructure);
            var contentfulResponse = new ContentfulResponse(deserialisedContentfulData);

            contentfulResponse.Items.Should().NotBeEmpty();
        }

        [Fact]
        public void CreatesContentfulResponseWithNoEntries()
        {
            var contentfulResponseNoEntries =
                "{\r\n  \"items\": [\r\n  {}\r\n  ],\r\n  \"includes\": {\r\n    \"Asset\": [\r\n  {}\r\n ]\r\n  }\r\n}";
            var deserialisedContentfulData = JsonConvert.DeserializeObject<dynamic>(contentfulResponseNoEntries);
            var contentfulResponse = new ContentfulResponse(deserialisedContentfulData);

            contentfulResponse.Items.Should().NotBeEmpty();
        }

        [Fact]
        public void CreatesContentfulResponseForNonExistentEntry()
        {
            var contentfulResponseNoEntries =
                "{\r\n  \"items\": []}";
            var deserialisedContentfulData = JsonConvert.DeserializeObject<dynamic>(contentfulResponseNoEntries);
            var contentfulResponse = new ContentfulResponse(deserialisedContentfulData);

            contentfulResponse.Items.Should().BeEmpty();
        }

        [Fact]
        public void RetrievesAListOfEntriesFromAListOfReferences()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ReferenceListOfTopics.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var references =
                "{\"fields\": {\r\n\"featuredTopics\": [\r\n{\r\n\"sys\": {\r\n\"type\": \"Link\",\r\n\"linkType\": \"Entry\",\r\n\"id\": \"6edPPmxkUEOWOiwQ8KSiYi\"\r\n}\r\n},\r\n{\r\n\"sys\": {\r\n\"type\": \"Link\",\r\n\"linkType\": \"Entry\",\r\n\"id\": \"r0wXv8StDECgWMaeOeECE\"\r\n}\r\n}\r\n]\r\n}}";
            var dynamicReferenceList = JsonConvert.DeserializeObject<dynamic>(references);

            var referenceList = dynamicReferenceList.fields.featuredTopics;
            IEnumerable<dynamic> entries = contentfulResponse.GetEntriesFor(referenceList);

            var items = entries.ToList();

            var slug = items.First().fields.slug.ToString();

            Assert.Equal("council-tax", slug);
        }

        [Fact]
        public void RetrievesAListOfAssetsFromAListOfReferences()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/Article/ArticleWithDocuments.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var references =
                "{\"fields\": {\r\n\"documents\": [\r\n{\r\n\"sys\": {\r\n\"type\": \"Link\",\r\n\"linkType\": \"Asset\",\r\n\"id\": \"5QG8oheTlKW2GSA80YgaYU\"\r\n}\r\n},\r\n{\r\n\"sys\": {\r\n\"type\": \"Link\",\r\n\"linkType\": \"Asset\",\r\n\"id\": \"6QG8oheTlKW2GSA80YgaYP\"\r\n}\r\n}\r\n]\r\n}}";
            var dynamicReferenceList = JsonConvert.DeserializeObject<dynamic>(references);

            var referenceList = dynamicReferenceList.fields.documents;
            IEnumerable<dynamic> assets = contentfulResponse.GetAssetsFor(referenceList);

            var items = assets.ToList();

            var title = items.First().fields.title.ToString();
            Assert.Equal("Test pdf", title);
        }
    }
}