using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Contentful.Core;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
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
    public class SmartAnswersRepositoryTests
    {
        private readonly SmartAnswersRepository _repository;
        private readonly Mock<IContentfulClientManager> _clientManager = new Mock<IContentfulClientManager>();
        private readonly Mock<IContentfulClient> _client = new Mock<IContentfulClient>();
        private readonly Mock<IContentfulFactory<ContentfulSmartAnswers, SmartAnswer>> _contentfulFactory;
        public SmartAnswersRepositoryTests()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _clientManager.Setup(_ => _.GetClient(It.IsAny<ContentfulConfig>())).Returns(_client.Object);

            _contentfulFactory = new Mock<IContentfulFactory<ContentfulSmartAnswers, SmartAnswer>>();

            _repository = new SmartAnswersRepository(config, _clientManager.Object, _contentfulFactory.Object);
        }

        [Fact]
        public void Get_ShouldReturnValidJson_WhenGivenValidSlug()
        {
            //Setup
            var slug = "smartAnswers";
            var collection = new ContentfulCollection<ContentfulSmartAnswers>();
            collection.Items =
                new List<ContentfulSmartAnswers>
                {
                    new ContentfulSmartAnswerBuilder().Build()
                };
            var smartAnswersModel = new SmartAnswer("smartAnswers", "questionJson1");

            _contentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulSmartAnswers>())).Returns(smartAnswersModel);
            var builder = new QueryBuilder<ContentfulSmartAnswers>().ContentTypeIs("smartAnswers").FieldEquals("fields.slug", slug).Include(1);
          
            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulSmartAnswers>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            //
            var response = _repository.Get(slug);
            var responseSmartAnswer = response.Result.Get<SmartAnswer>();

            //
            _client.Verify(_ => _.GetEntriesAsync(It.IsAny<QueryBuilder<ContentfulSmartAnswers>>(), It.IsAny<CancellationToken>()), Times.Once);
            response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            responseSmartAnswer.ShouldBeEquivalentTo(smartAnswersModel);
       }

        [Fact]
        public void Get_ShouldReturnNoValidJson_WhenSmartAnswersNotFound()
        {
            //Setup
            var slug = "smartAnswers";

            var builder = new QueryBuilder<ContentfulSmartAnswers>().ContentTypeIs("smartAnswers").FieldEquals("fields.slug", slug).Include(1);

            _client.Setup(o => o.GetEntriesAsync(It.Is<QueryBuilder<ContentfulSmartAnswers>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(null);

            //
            var response = _repository.Get(slug);
            var responseSmartAnswer = response.Result.Get<SmartAnswer>();

            //
            _client.Verify(_ => _.GetEntriesAsync(It.IsAny<QueryBuilder<ContentfulSmartAnswers>>(), It.IsAny<CancellationToken>()), Times.Once);
            response.Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseSmartAnswer.ShouldBeEquivalentTo(null);
        }
    }
}
