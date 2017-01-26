﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class TopicRepositoryTest
    {
        private readonly TopicRepository _repository;
        private readonly Topic _topic;
        private readonly Mock<IContentfulFactory<ContentfulTopic, Topic>> _topicFactory;
        private readonly Mock<Contentful.Core.IContentfulClient> _contentfulClient;

        public TopicRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _topic = new Topic("slug", "name", "teaser", "summary", "icon", "backgroundImage", new List<SubItem>(), new List<SubItem>(),
                new List<SubItem>(), new List<Crumb>(), new List<Alert>(), DateTime.MinValue, DateTime.MinValue, true, "test-id");

            _topicFactory = new Mock<IContentfulFactory<ContentfulTopic, Topic>>();
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<Contentful.Core.IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            _repository = new TopicRepository(config, contentfulClientManager.Object, _topicFactory.Object);
        }

        [Fact]
        public void GetsTopicByTopicSlug()
        {
            const string slug = "a-slug";
            var contentfulTopic = new ContentfulTopicBuilder().Slug(slug).Build();
            var builder = new QueryBuilder().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulTopic>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()), 
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulTopic> { contentfulTopic });

            _topicFactory.Setup(o => o.ToModel(contentfulTopic)).Returns(_topic);

            var response = AsyncTestHelper.Resolve(_repository.GetTopicByTopicSlug(slug));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var topic = response.Get<Topic>();
            topic.ShouldBeEquivalentTo(_topic);
        }

        [Fact]
        public void GetsNotFoundIfTopicDoesNotExist()
        {
            const string slug = "not-found";
            var builder = new QueryBuilder().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulTopic>(It.Is<QueryBuilder>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(new List<ContentfulTopic>());

            var response = AsyncTestHelper.Resolve(_repository.GetTopicByTopicSlug(slug));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be($"No topic found for '{slug}'");
        }
    }
}