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
using Microsoft.Extensions.Logging;
using StockportContentApiTests.Unit.Builders;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ContactUsAreaRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly ContactUsAreaRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _topicFactory;
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
        private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory;
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

        private readonly Mock<ITimeProvider> _timeprovider;
        private readonly Mock<ICache> _cacheWrapper;
        private readonly Mock<IConfiguration> _configuration;

        public ContactUsAreaRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();
            _topicFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _timeprovider = new Mock<ITimeProvider>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty));

            var contentfulFactory = new ContactUsAreaContentfulFactory(_topicFactory.Object, HttpContextFake.GetHttpContextFake(), _crumbFactory.Object, _timeprovider.Object, _alertFactory.Object);

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _cacheWrapper = new Mock<ICache>();

            var _logger = new Mock<ILogger<EventRepository>>();
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");

            _repository = new ContactUsAreaRepository(config, contentfulClientManager.Object, contentfulFactory);
        }

        [Fact]
        public void ItGetsContactUsArea()
        {
            // Arrange
            const string slug = "contactusarea";

            var collection = new ContentfulCollection<ContentfulContactUsArea>();
            var rawContactUsArea = new ContentfulContactUsAreaBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulContactUsArea> { rawContactUsArea };

            var builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactusarea").FieldEquals("fields.slug", slug).Include(3);

            _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsArea>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

//            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetContactUsArea());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ItReturnsBreadcrumbs()
        {
            // Arrange
            const string slug = "contactusarea";
            var crumb = new Crumb("title", "slug", "type");
            var collection = new ContentfulCollection<ContentfulContactUsArea>();
            var rawContactUsArea = new ContentfulContactUsAreaBuilder().Slug(slug)
                .Breadcrumbs(new List<ContentfulReference>()
                            { new ContentfulReference() {Title = crumb.Title, Slug = crumb.Title, Sys = new SystemProperties() {Type = "Entry" }},
                            })
                .Build();
            collection.Items = new List<ContentfulContactUsArea> { rawContactUsArea };

            var builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactusarea").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsArea>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetContactUsArea());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var contactUsArea = response.Get<ContactUsArea>();

            contactUsArea.Breadcrumbs.First().Should().Be(crumb);
        }
    }
}
