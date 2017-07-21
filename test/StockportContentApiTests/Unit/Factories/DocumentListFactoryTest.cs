using System;
using System.Collections.Generic;
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
    public class DocumentListFactoryTest : TestingBaseClass
    {
        private readonly DocumentListFactory _documentListFactory;

        public DocumentListFactoryTest()
        {
            _documentListFactory = new DocumentListFactory();
        }

        [Fact]
        public void ItBuildsBreadcrumbsFromContentfulResponse()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Article.ArticleWithDocuments.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();
            IEnumerable<Document> documents = _documentListFactory.BuildFromReferences(article.fields.documents, contentfulResponse);

            documents.Should().HaveCount(2);
            var firstDocument = documents.First();
            firstDocument.Title.Should().Be("metroshuttle route map");
            firstDocument.Size.Should().Be(674192);
            firstDocument.Url.Should().Be("document.pdf");
            firstDocument.LastUpdated.Should().Be(new DateTime(2016, 10, 5, 11, 9, 48));
            firstDocument.FileName.Should().Be("Stockport-Metroshuttle.pdf");

            var secondDocument = documents.ToList()[1];
            secondDocument.Title.Should().Be("A pdf");
            secondDocument.Size.Should().Be(4563);
            secondDocument.Url.Should().Be("document.pdf");
            secondDocument.LastUpdated.Should().Be(new DateTime(2016, 12, 05, 11, 09, 48));
            secondDocument.FileName.Should().Be("a-pdf.pdf");
        }
    }
}
