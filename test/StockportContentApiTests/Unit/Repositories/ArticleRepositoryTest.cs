using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using StockportContentApiTests.Unit.Fakes;
using Xunit;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Client;
using StockportContentApiTests.Unit.Builders;
using File = System.IO.File;
using IContentfulClient = Contentful.Core.IContentfulClient;
using Contentful.Core.Search;
using System.Threading;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ArticleRepositoryTest
    {       
        private readonly ArticleRepository _repository;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private Mock<IVideoRepository> _videoRepository;
        private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory;
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
        private readonly Mock<IContentfulFactory<ContentfulArticle, Topic>> _parentTopicFactory;
        private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

        public ArticleRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();          
            var documentFactory = new DocumentContentfulFactory();
            _videoRepository = new Mock<IVideoRepository>();
            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns(string.Empty);
            _mockTimeProvider = new Mock<ITimeProvider>();

            _sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            _parentTopicFactory = new Mock<IContentfulFactory<ContentfulArticle, Topic>>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();

            var contentfulFactory = new ArticleContentfulFactory(
                _sectionFactory.Object, 
                _crumbFactory.Object, 
                _profileFactory.Object, 
                _parentTopicFactory.Object,
                documentFactory, 
                _videoRepository.Object,
                _mockTimeProvider.Object,
                _alertFactory.Object
                );

           var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _repository = new ArticleRepository(config, contentfulClientManager.Object, _mockTimeProvider.Object, contentfulFactory, new ArticleSiteMapContentfulFactory(), _videoRepository.Object);
        }
        
        [Fact]
        public void GetsArticle()
        {
            const string slug = "unit-test-article";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

            var collection = new ContentfulCollection<ContentfulArticle>();
            var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulArticle> { rawArticle };

            var builder = new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulArticle>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var response = AsyncTestHelper.Resolve(_repository.GetArticle(slug));           
           
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GetAllArticleSlugForSitemap()
        {
            var collection = new ContentfulCollection<ContentfulArticleForSiteMap>();
            collection.Items = new List<ContentfulArticleForSiteMap> {new ContentfulArticleForSiteMap() {Slug = "slug1"}, new ContentfulArticleForSiteMap() { Slug = "slug2" }, new ContentfulArticleForSiteMap() { Slug = "slug3" } };
            var builder = new QueryBuilder<ContentfulArticleForSiteMap>().ContentTypeIs("article").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulArticleForSiteMap>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            var response = AsyncTestHelper.Resolve(_repository.Get());
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var responseArticle = response.Get<List<ArticleSiteMap>>();
            responseArticle.Count.Should().Be(collection.Items.Count());
        }

        [Fact]
        public void GetsNotFoundForAnArticleThatDoesNotExist()
        {           
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

            var collection = new ContentfulCollection<ContentfulArticle>();
            collection.Items = new List<ContentfulArticle>();

            _contentfulClient.Setup(o => o.GetEntriesAsync(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);
            var response = AsyncTestHelper.Resolve(_repository.GetArticle("blah"));           

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No article found for 'blah'");
        }

        [Fact]
        public void Gets404ForNewsOutsideOfSunriseDate()
        {
            const string slug = "unit-test-article";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 01, 01));

            var collection = new ContentfulCollection<ContentfulArticle>();
            var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulArticle> { rawArticle };

            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulArticle>(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);
           
            HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void Gets404ForNewsOutsideOfSunsetDate()
        {
            const string slug = "unit-test-article";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 08, 01));

            var collection = new ContentfulCollection<ContentfulArticle>();
            var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulArticle> { rawArticle };

            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulArticle>(It.IsAny<QueryBuilder<ContentfulArticle>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact]
        public void ReturnsValidSunsetAndSunriseDateWhenDateInRange()
        {
            const string slug = "unit-test-article";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 08, 01));

            var collection = new ContentfulCollection<ContentfulArticle>();
            var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulArticle> { rawArticle };

            var builder = new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulArticle>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ReturnsArticleWithInlineAlerts()
        {            
            // Arrange
            const string slug = "unit-test-article-with-inline-alerts";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));
            List<ContentfulAlert> alertsInline = new List<ContentfulAlert> { new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(2017, 05, 01),
                SunsetDate = new DateTime(2017, 07, 01),
                Sys = new SystemProperties() { Type = "Entry" }
            }
        };
            var collection = new ContentfulCollection<ContentfulArticle>();
            var rawArticle = new ContentfulArticleBuilder().Slug(slug).AlertsInline(alertsInline).Build();
            collection.Items = new List<ContentfulArticle> { rawArticle };
            var builder = new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(
                    It.Is<QueryBuilder<ContentfulArticle>>(
                        q => q.Build() == builder.Build()), 
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetArticle(slug));

            // Assert
            var article = response.Get<Article>();
            article.AlertsInline.Should().NotBeNull();
        }

        [Fact]
        public void ReturnsArticleWithASectionThatHasAnInlineAlert()
        {
            // Arrange
            const string slug = "unit-test-article-with-section-with-inline-alerts";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));
            var alert = new Alert("title", "subHeading", "body", "severity",
                        new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                        new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc));
            var collection = new ContentfulCollection<ContentfulArticle>();
            var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulArticle> { rawArticle };
            var builder = new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", slug).Include(3);
            _contentfulClient.Setup(o => o.GetEntriesAsync(
                    It.Is<QueryBuilder<ContentfulArticle>>(
                        q => q.Build() == builder.Build()),
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);
            _sectionFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSection>())).Returns(new Section(
                "title",
                "section-with-inline-alerts",
                "body",
                new List<Profile>(),
                new List<Document>(),
                new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
                new List<Alert> { alert }));

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetArticle(slug));

            // Assert
            var article = response.Get<Article>();
            article.Sections[0].AlertsInline.Should().Equal(alert);
        }

        private static Article EmptyArticle()
        {
            return new Article("", "", "", "", "", "", "", new List<Section>(), new List<Crumb>(),
                new List<Alert>(), new List<Profile>(), new NullTopic(), new List<Document>(),
                new DateTime(2016, 10, 1), new DateTime(2016, 10, 31), false, new NullLiveChat(), new List<Alert>());
        }
    }
}