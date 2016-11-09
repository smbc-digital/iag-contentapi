using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApiTests.Unit.Fakes;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class TopicRepositoryTest
    {
        private readonly Mock<IFactory<Topic>> _mockTopicBuilder;
        private readonly FakeHttpClient _httpClient = new FakeHttpClient();
        private readonly TopicRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly Topic _topic;

        public TopicRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _topic = new Topic("slug", "name", "teaser", "summary", "icon", "backgroundImage", new List<SubItem>(), new List<SubItem>(),
                new List<SubItem>(), new List<Crumb>(), new List<Alert>(), DateTime.MinValue, DateTime.MinValue, true, "test-id");

            _mockTopicBuilder = new Mock<IFactory<Topic>>();

            _repository = new TopicRepository(config, _httpClient, _mockTopicBuilder.Object);
        }

        [Fact]
        public void GetsTopicByTopicSlug()
        {
            _httpClient.For($"{MockContentfulApiUrl}&content_type=topic&include=1&fields.slug=healthy-living")
                .Return(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Topic.json")));

            _mockTopicBuilder.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                 .Returns(_topic);

            var response = AsyncTestHelper.Resolve(_repository.GetTopicByTopicSlug("healthy-living"));
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var topic = response.Get<Topic>();

            topic.Name.Should().Be(_topic.Name);
            topic.Slug.Should().Be(_topic.Slug);
            topic.EmailAlerts.Should().Be(_topic.EmailAlerts);
            topic.EmailAlertsTopicId.Should().Be(_topic.EmailAlertsTopicId);
        }

        [Fact]
        public void GetsNotFoundIfTopicDoesNotExist()
        {
            _httpClient.For($"{MockContentfulApiUrl}&content_type=topic&include=1&fields.slug=blah")
                .Return(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetTopicByTopicSlug("blah"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No topic found for 'blah'");
        }
    }
}