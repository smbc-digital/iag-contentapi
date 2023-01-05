using System.Net;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;

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
        private readonly Mock<ILogger<ShowcaseRepository>> _mockLogger;

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
            _mockLogger = new Mock<ILogger<ShowcaseRepository>>();
            _timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();
            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false));

            var _videoFactory = new Mock<IContentfulFactory<ContentfulVideo, Video>>();

            var _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();

            var _triviaFactory = new Mock<IContentfulFactory<ContentfulTrivia, Trivia>>();

            var callToActionBanner = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();
            callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
                new CallToActionBanner
                {
                    Title = "title",
                    AltText = "altText",
                    ButtonText = "button text",
                    Image = "url",
                    Link = "url"
                });

            var spotlightBannerFactory = new Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>>();

            var contentfulFactory = new ShowcaseContentfulFactory(_topicFactory.Object, _crumbFactory.Object, _timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

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

            _repository = new ShowcaseRepository(config, contentfulFactory, contentfulClientManager.Object, newsListFactory.Object, eventRepository, _mockLogger.Object);
        }

        [Fact]
        public void ItGetsShowcase()
        {
            // Arrange
            const string slug = "unit-test-showcase";

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
            var events = new List<ContentfulEvent> { rawEvent };

            var modelledEvent = new Event("title", "event-slug", "", "", "", "", "", "", DateTime.MaxValue, "", "", 1, EventFrequency.None, null, "", null, new List<string>(), null, false, "", DateTime.MinValue, new List<string>(), null, null, new List<EventCategory> { new EventCategory("event", "slug", "icon") }, null, null, null, null);
            _eventFactory.Setup(e => e.ToModel(It.IsAny<ContentfulEvent>())).Returns(modelledEvent);

            var collection = new ContentfulCollection<ContentfulShowcase>();
            var rawShowcase = new ContentfulShowcaseBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulShowcase> { rawShowcase };

            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);

            _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
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
            _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulShowcase>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

            var rawEvent = new ContentfulEventBuilder().Slug(slug).EventDate(new DateTime(2017, 4, 1)).Build();
            var events = new List<ContentfulEvent> { rawEvent };
            _cacheWrapper.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == "event-all"), It.IsAny<Func<Task<IList<ContentfulEvent>>>>(), It.Is<int>(s => s == 60))).ReturnsAsync(events);
            var modelledEvent = new Event("title", "event-slug", "", "", "", "", "", "", DateTime.MaxValue, "", "", 1, EventFrequency.None, null, "", null, new List<string>(), null, false, "", DateTime.MinValue, new List<string>(), null, null, new List<EventCategory> { new EventCategory("event", "slug", "icon") }, null, null, null, null);
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
