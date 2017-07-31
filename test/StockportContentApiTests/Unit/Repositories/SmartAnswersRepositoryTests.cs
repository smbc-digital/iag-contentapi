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
using Microsoft.Extensions.Logging;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
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
        private readonly Mock<ILogger<SmartAnswersRepository>> _logger = new Mock<ILogger<SmartAnswersRepository>>();
        private readonly Mock<ICache> _cache = new Mock<ICache>();

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

            _repository = new SmartAnswersRepository(config, _clientManager.Object, _contentfulFactory.Object, _cache.Object, _logger.Object);
        }

        [Fact]
        public void Get_ShouldReturnValidJson_WhenGivenValidSlug()
        {
            //Setup
            var slug = "smartAnswers";
            var smartAnswer = new ContentfulSmartAnswerBuilder().Build();

            var smartAnswersModel = new SmartAnswer("title", "smartAnswers", "questionJson1");

            _contentfulFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulSmartAnswers>())).Returns(smartAnswersModel);

            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"smart-{slug}"), It.IsAny<Func<Task<ContentfulSmartAnswers>>>())).ReturnsAsync(smartAnswer);

            //
            var response = _repository.Get(slug);
            var responseSmartAnswer = response.Result.Get<SmartAnswer>();

            //
            _cache.Verify(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"smart-{slug}"), It.IsAny<Func<Task<ContentfulSmartAnswers>>>()), Times.Once);
            response.Result.StatusCode.Should().Be(HttpStatusCode.OK);
            responseSmartAnswer.ShouldBeEquivalentTo(smartAnswersModel);
       }

        [Fact]
        public void Get_ShouldReturnNoValidJson_WhenSmartAnswersNotFound()
        {
            //Setup
            var slug = "smartAnswers";

            _cache.Setup(o => o.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"smart-{slug}"), It.IsAny<Func<Task<ContentfulSmartAnswers>>>())).ReturnsAsync(null);

            //
            var response = _repository.Get(slug);
            var responseSmartAnswer = response.Result.Get<SmartAnswer>();

            //
            _cache.Verify(_ => _.GetFromCacheOrDirectlyAsync(It.Is<string>(s => s == $"smart-{slug}"), It.IsAny<Func<Task<ContentfulSmartAnswers>>>()), Times.Once);
            response.Result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            responseSmartAnswer.ShouldBeEquivalentTo(null);
        }
    }
}
