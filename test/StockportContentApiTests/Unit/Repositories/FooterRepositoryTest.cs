using System.Collections.Generic;
using System.IO;
using System.Net;
using FluentAssertions;
using Moq;
using StockportContentApi;
using StockportContentApi.Config;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class FooterRepositoryTest
    {
        private readonly ContentfulConfig _config;
        private readonly Mock<IHttpClient> _mockHttpClient;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";

        public FooterRepositoryTest()
        {
            _config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();
            _mockHttpClient = new Mock<IHttpClient>();
        }

        [Fact]
        public void ShouldReturnAFooter()
        {
            var builderFooter = new Footer("Title", "a-slug", "Copyright", new List<SubItem>(),
                new List<SocialMediaLink>());

            _mockHttpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=footer&include=1"))
                .ReturnsAsync(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Footer.json")));

            var mockFooterFactory = new Mock<IFactory<Footer>>();
            mockFooterFactory.Setup(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()))
                .Returns(builderFooter);

            var repository = new FooterRepository(_config, _mockHttpClient.Object, mockFooterFactory.Object);

            var footer = AsyncTestHelper.Resolve(repository.GetFooter());

            footer.Get<Footer>().Should().Be(builderFooter);
            footer.StatusCode.Should().Be(HttpStatusCode.OK);
            mockFooterFactory.Verify(o => o.Build(It.IsAny<object>(), It.IsAny<ContentfulResponse>()), Times.Once);
        }

        public void ShouldReturn404IfNoEntryExists()
        {
            _mockHttpClient.Setup(o => o.Get($"{MockContentfulApiUrl}&content_type=footer&include=1"))
                .ReturnsAsync(HttpResponse.Failure(HttpStatusCode.NotFound, "error message"));

            var repository = new FooterRepository(_config, _mockHttpClient.Object, new Mock<IFactory<Footer>>().Object);

            var footer = AsyncTestHelper.Resolve(repository.GetFooter());

            footer.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
