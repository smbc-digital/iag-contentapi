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
using StockportContentApi.Factories;
using Microsoft.Extensions.Logging;
using StockportContentApiTests.Unit.Builders;
using Microsoft.Extensions.Caching.Memory;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ShowcaseRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly ShowcaseRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _topicFactory;
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;
        private readonly Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>> _eventHomepageFactory;
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

        private readonly Mock<ITimeProvider> _timeprovider;
        private readonly Mock<ICache> _cacheWrapper;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<IHttpContextAccessor> _httpContextAccessor;

        public ShowcaseRepositoryTest()
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
            _eventHomepageFactory = new Mock<IContentfulFactory<ContentfulEventHomepage, EventHomepage>>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _httpContextAccessor = new Mock<IHttpContextAccessor>();
            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var consultationFactory = new Mock<IContentfulFactory<ContentfulConsultation, Consultation>>();
            consultationFactory.Setup(o => o.ToModel(It.IsAny<ContentfulConsultation>())).Returns(new Consultation("title", DateTime.Now, "https://www.stockport.gov.uk/link"));

            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();
            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url"));

            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty));

            var _keyFactFactory = new Mock<IContentfulFactory<ContentfulKeyFact, KeyFact>>();

            var _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();

            var contentfulFactory = new ShowcaseContentfulFactory(_topicFactory.Object, _crumbFactory.Object, _timeprovider.Object, consultationFactory.Object, socialMediaFactory.Object, _alertFactory.Object, _keyFactFactory.Object, _profileFactory.Object, HttpContextFake.GetHttpContextFake());

            var eventListFactory = new Mock<IContentfulFactory<List<ContentfulEvent>, List<Event>>>();

            var newsListFactory = new Mock<IContentfulFactory<ContentfulNews, News>>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
            _cacheWrapper = new Mock<ICache>();

            var _logger = new Mock<ILogger<EventRepository>>();
            _configuration = new Mock<IConfiguration>();
            _configuration.Setup(_ => _["redisExpiryTimes:Events"]).Returns("60");

            var eventRepository = new EventRepository(config, contentfulClientManager.Object, _timeprovider.Object, _eventFactory.Object, _eventHomepageFactory.Object, _cacheWrapper.Object, _logger.Object, _configuration.Object);

            

            _repository = new ShowcaseRepository(config, contentfulFactory, contentfulClientManager.Object, eventListFactory.Object, newsListFactory.Object, _timeprovider.Object, eventRepository);
        }

        [Fact]
        public void ItGetsShowcase()
        {
            // Arrange
            const string slug = "unit-test-showcase";

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
            var events = new List<ContentfulEvent> { rawEvent };

            var modelledEvent = new Event("title", "event-slug", "", "", "", "", "", "", DateTime.MaxValue, "", "", 1, EventFrequency.None,  null, "", null, new List<string>(), null, false, "", DateTime.MinValue, new List<string>(), null, null, new List<EventCategory> { new EventCategory("event", "slug", "icon") }, null, null,null);
            _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

            var collection = new ContentfulCollection<ContentfulShowcase>();
            var rawShowcase = new ContentfulShowcaseBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulShowcase> { rawShowcase };

            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);

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
            var collection = new ContentfulCollection<ContentfulShowcase>();
            var rawShowcase = new ContentfulShowcaseBuilder().Slug(slug)
                .Breadcrumbs(new List<ContentfulReference>()
                            { new ContentfulReference() {Title = crumb.Title, Slug = crumb.Title, Sys = new SystemProperties() {Type = "Entry" }},
                            })
                .Build();
            collection.Items = new List<ContentfulShowcase> { rawShowcase };

            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
            var events = new List<ContentfulEvent> { rawEvent };
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);
            var modelledEvent = new Event("title", "event-slug", "", "", "", "", "", "", DateTime.MaxValue, "", "", 1, EventFrequency.None, null, "", null, new List<string>(), null, false, "", DateTime.MinValue, new List<string>(), null, null, new List<EventCategory> { new EventCategory("event","slug", "icon")}, null, null,null);
            _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetShowcases(slug));

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var showcase = response.Get<Showcase>();

            showcase.Breadcrumbs.First().Should().Be(crumb);
        }
    }
}
