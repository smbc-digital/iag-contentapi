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

namespace StockportContentApiTests.Unit.Repositories
{
    public class ShowcaseRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly ShowcaseRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulSubItem, SubItem>> _topicFactory;
        private readonly Mock<IContentfulFactory<ContentfulCrumb, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory;

        private readonly Mock<ITimeProvider> _timeprovider;
        private readonly ICacheWrapper _cacheWrapper;

        public ShowcaseRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();
            _topicFactory = new Mock<IContentfulFactory<ContentfulSubItem, SubItem>>();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulCrumb, Crumb>>();
            _timeprovider = new Mock<ITimeProvider>();

            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var consultationFactory = new Mock<IContentfulFactory<ContentfulConsultation, Consultation>>();
            consultationFactory.Setup(o => o.ToModel(It.IsAny<ContentfulConsultation>())).Returns(new Consultation("title", DateTime.Now, "https://www.stockport.gov.uk/link"));

            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();
            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url"));

            var contentfulFactory = new ShowcaseContentfulFactory(_topicFactory.Object, _crumbFactory.Object, _timeprovider.Object, consultationFactory.Object, socialMediaFactory.Object);

            var eventListFactory = new Mock<IContentfulFactory<List<ContentfulEvent>, List<Event>>>();

            var newsListFactory = new Mock<IContentfulFactory<ContentfulNews, News>>();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _eventFactory = new Mock<IContentfulFactory<ContentfulEvent, Event>>();
            var _eventCategoriesFactory = new Mock<IEventCategoriesFactory>();

            var memoryCache = new MemoryCache(new MemoryCacheOptions());
            _cacheWrapper = new CacheWrapper(memoryCache);

            var _logger = new Mock<ILogger<EventRepository>>();
            var eventRepository = new EventRepository(config, _httpClient.Object, contentfulClientManager.Object, _timeprovider.Object, _eventFactory.Object, _eventCategoriesFactory.Object, _cacheWrapper, _logger.Object);

            _repository = new ShowcaseRepository(config, contentfulFactory, contentfulClientManager.Object, eventListFactory.Object, newsListFactory.Object, _timeprovider.Object, eventRepository);
        }

        [Fact]
        public void ItGetsShowcase()
        {
            // Arrange
            const string slug = "unit-test-showcase";

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
            var events = new List<ContentfulEvent> { rawEvent };
            _contentfulClient.Setup(
                   o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(events);

            var modelledEvent = new Event("title", "event-slug", "", "", "", "", "", "", DateTime.MaxValue, "", "", 1, EventFrequency.None,  null, "", null, new List<string>(), null, false, "", DateTime.MinValue, new List<string>(), null, null);
            _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

            var rawShowcase = new ContentfulShowcaseBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);

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
                .Breadcrumbs(new List<ContentfulCrumb>()
                            { new ContentfulCrumb() {Title = crumb.Title, Slug = crumb.Title, Sys = new SystemProperties() {Type = "Entry" }},
                            })
                .Build();

            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulShowcase> { rawShowcase });

            _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulCrumb>())).Returns(crumb);

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
            var events = new List<ContentfulEvent> { rawEvent };
            _contentfulClient.Setup(
                   o => o.GetEntriesAsync<ContentfulEvent>(It.IsAny<QueryBuilder<ContentfulEvent>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(events);

            var modelledEvent = new Event("title", "event-slug", "", "", "", "", "", "", DateTime.MaxValue, "", "", 1, EventFrequency.None, null, "", null, new List<string>(), null, false, "", DateTime.MinValue, new List<string>(), null, null);
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
