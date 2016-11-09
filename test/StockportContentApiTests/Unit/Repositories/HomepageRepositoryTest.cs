using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;
using Moq;
using StockportContentApi;

namespace StockportContentApiTests.Unit.Repositories
{
    public class HomepageRepositoryTest
    {
        private readonly Mock<IHttpClient> _httpClient;
        private readonly HomepageRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly Mock<IFactory<Homepage>> _mockHomepageBuilder;

        public HomepageRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            _httpClient = new Mock<IHttpClient>();
            _mockHomepageBuilder = new Mock<IFactory<Homepage>>();
            _repository = new HomepageRepository(config, _httpClient.Object, _mockHomepageBuilder.Object);
        }

        [Fact]
        public void ItGetsHomepage()
        {
            _httpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=homepage&include=2"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Homepage.json")));

            _mockHomepageBuilder.Setup(
                    o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(new Homepage(new List<string>(), string.Empty, string.Empty, new List<SubItem>(), new List<Topic>(), new List<Alert>(), new List<CarouselContent>(), string.Empty, string.Empty));

            var response = AsyncTestHelper.Resolve(_repository.Get());
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}
