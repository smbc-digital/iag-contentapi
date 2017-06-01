using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using StockportContentApi.Client;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ShowcaseRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly ShowcaseRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>> _topicFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>> _crumbFactory;
        private readonly Mock<ITimeProvider> _timeprovider;

        public ShowcaseRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();
            _topicFactory = new Mock<IContentfulFactory<Entry<ContentfulSubItem>, SubItem>>();
            _crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            _timeprovider = new Mock<ITimeProvider>();

            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var consultationFactory = new Mock<IContentfulFactory<ContentfulConsultation, Consultation>>();
            consultationFactory.Setup(o => o.ToModel(It.IsAny<ContentfulConsultation>())).Returns(new Consultation("title", DateTime.Now, "https://www.stockport.gov.uk/link"));

            var contentfulFactory = new ShowcaseContentfulFactory(_topicFactory.Object, _crumbFactory.Object, _timeprovider.Object, consultationFactory.Object);

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _repository = new ShowcaseRepository(config, contentfulFactory, contentfulClientManager.Object);
        }

        [Fact]
        public void ItGetsShowcase()
        {
            // Arrange
            const string slug = "unit-test-showcase";

            var rawShowcase = new ContentfulShowcaseBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<Entry<ContentfulShowcase>>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulShowcase> { rawShowcase });

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetShowcases(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ItReturnsBreadcrumbs()
        {
            // Arrange
            const string slug = "unit-test-showcase-crumbs";
            var crumb = new Crumb("title", "slug", "type");
            var rawShowcase = new ContentfulShowcaseBuilder().Slug(slug)
                .Breadcrumbs(new List<Entry<ContentfulCrumb>>()
                            { new Entry<ContentfulCrumb>()
                                {
                                    Fields = new ContentfulCrumb() {Title = crumb.Title, Slug = crumb.Title},
                                    SystemProperties = new SystemProperties() {Type = "Entry" }
                                },
                            })
                .Build();

            var builder = new QueryBuilder<Entry<ContentfulShowcase>>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulShowcase> { rawShowcase });

            _crumbFactory.Setup(o => o.ToModel(It.IsAny<Entry<ContentfulCrumb>>())).Returns(crumb);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetShowcases(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var showcase = response.Get<Showcase>();

            showcase.Breadcrumbs.First().Should().Be(crumb);
        }
    }
}
