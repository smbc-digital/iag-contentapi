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
    public class BreadcrumbFactoryTest : TestingBaseClass
    {
        private readonly BreadcrumbFactory _breadcrumbFactory;

        public BreadcrumbFactoryTest()
        {
            _breadcrumbFactory = new BreadcrumbFactory();
        }

        [Fact]
        public void ItBuildsBreadcrumbsFromContentfulResponse()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Article.ArticleWithBreadcrumbs.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();

            IEnumerable<Crumb> breadcrumbs = _breadcrumbFactory.BuildFromReferences(article.fields.breadcrumbs, contentfulResponse);

            breadcrumbs.Should().HaveCount(2);
            breadcrumbs.First().Slug.Should().Be("slug");
            breadcrumbs.First().Type.Should().Be("topic");
            breadcrumbs.Last().Title.Should().Be("title 2");
            breadcrumbs.Last().Type.Should().Be("article");
        }

        [Fact]
        public void ItBuildsEmptyListWhenNoBreadcrumbs()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Article.ArticleNoBreadcrumbs.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            IEnumerable<Crumb> breadcrumbs = _breadcrumbFactory.BuildFromReferences(null, contentfulResponse);
            breadcrumbs.Should().BeEmpty();
        }
    }
}