﻿using System;
using System.Collections.Generic;
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
        //private readonly FakeHttpClient _httpClient = new FakeHttpClient();
       // private readonly Mock<IHttpClient> _httpClient;
        //private readonly ArticleRepository _repository;
        //private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        //private Mock<IFactory<Article>> _mockArticleBuilder;
        //private Mock<IVideoRepository> _videoRepository;
        //private Mock<ITimeProvider> _mockTimeProvider;
        //private readonly DateTime _sunriseDate = new DateTime(2016, 08, 01);
        //private readonly DateTime _sunsetDate = new DateTime(2016, 08, 10);


        private readonly ArticleRepository _repository;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IHttpClient> _httpClient;       
        private readonly Mock<IContentfulFactory<ContentfulArticle, Article>> _articleFactory;
        private Mock<IVideoRepository> _videoRepository;
        private readonly DateTime _sunriseDate = new DateTime(2016, 08, 01);
        private readonly DateTime _sunsetDate = new DateTime(2016, 08, 10);
        private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory;
        private readonly Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>> _crumbFactory;
        private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
        private readonly Mock<IContentfulFactory<ContentfulTopic, Topic>> _topicFactory;


        public ArticleRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();          
            var documentFactory = new DocumentContentfulFactory();
            _videoRepository = new Mock<IVideoRepository>();
            _httpClient = new Mock<IHttpClient>();
            _videoRepository.Setup(o => o.Process(It.IsAny<string>())).Returns(string.Empty);
            _mockTimeProvider = new Mock<ITimeProvider>();

            _sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();
            _crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
            _topicFactory = new Mock<IContentfulFactory<ContentfulTopic, Topic>>();

                var contentfulFactory = new ArticleContentfulFactory(_sectionFactory.Object, _crumbFactory.Object, _profileFactory.Object, _topicFactory.Object, documentFactory, _videoRepository.Object);

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _repository = new ArticleRepository(config, _httpClient.Object, contentfulClientManager.Object, _mockTimeProvider.Object, contentfulFactory, _videoRepository.Object);
        }
        
        [Fact]
        public void GetsArticle()
        {
            const string slug = "unit-test-article";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

            var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", slug).Include(2);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulArticle>(It.Is<QueryBuilder<ContentfulArticle>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulArticle> { rawArticle });

            var response = AsyncTestHelper.Resolve(_repository.GetArticle(slug));           
           
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void GetsNotFoundForAnArticleThatDoesNotExist()
        {
            const string slug = "test";
            _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

            var rawArticle = new ContentfulArticleBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulArticle>().ContentTypeIs("article").FieldEquals("fields.slug", slug).Include(2);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulArticle>(It.Is<QueryBuilder<ContentfulArticle>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulArticle> { rawArticle });

            var response = AsyncTestHelper.Resolve(_repository.GetArticle("blah"));           

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No article found for 'blah'");
        }

        //[Fact]
        //public void Gets404ForNewsOutsideOfSunriseDate()
        //{
        //    _mockArticleBuilder.Setup(
        //           o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
        //       .Returns(EmptyArticle());

        //    _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2015, 12, 01));

        //    _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=unit-test-article"))
        //         .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));
        //    //var test = _repository.GetArticle("unit-test-article");
        //    HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        //    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //}

        //[Fact]
        //public void Gets404ForNewsOutsideOfSunsetDate()
        //{
        //    _mockArticleBuilder.Setup(
        //           o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
        //       .Returns(EmptyArticle());

        //    _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2019, 12, 01));

        //    _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=unit-test-article"))
        //         .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));

        //    HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        //    response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        //}

        //[Fact]
        //public void ReturnsValidSunsetAndSunriseDateWhenDateInRange()
        //{
        //    _mockArticleBuilder.Setup(
        //           o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
        //       .Returns(EmptyArticle());

        //    _mockTimeProvider.Setup(o => o.Now()).Returns(new DateTime(2016, 10, 15));

        //    _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=article&include=2&fields.slug=unit-test-article"))
        //         .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Article.json")));

        //    HttpResponse response = AsyncTestHelper.Resolve(_repository.GetArticle("unit-test-article"));

        //    response.StatusCode.Should().Be(HttpStatusCode.OK);
        //}

        private static Article EmptyArticle()
        {
            return new Article("", "", "", "", "", "", "", new List<Section>(), new List<Crumb>(),
                new List<Alert>(), new List<Profile>(), new NullTopic(), new List<Document>(),
                new DateTime(2016, 10, 1), new DateTime(2016, 10, 31), false, new NullLiveChat());
        }
    }
}