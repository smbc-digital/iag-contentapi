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
    public class ProfileRepositoryTest
    {
        private readonly ProfileRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly FakeHttpClient _httpClient = new FakeHttpClient();

        public ProfileRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            var mockBreadcrumbFactory = new Mock<IBuildContentTypesFromReferences<Crumb>>();
            mockBreadcrumbFactory.Setup(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Crumb>() { new NullCrumb() });

            _repository = new ProfileRepository(config, _httpClient, new ProfileFactory(mockBreadcrumbFactory.Object));
        }

        [Fact]
        public void GetsProfileForProfileSlug()
        {
            _httpClient.For($"{MockContentfulApiUrl}&content_type=profile&include=1&fields.slug=test-profile")
                .Return(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/Profile.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetProfile("test-profile"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            response.Get<Profile>().Slug.Should().Be("test-profile");
        }

        [Fact]
        public void Return404WhenProfileWhenItemsDontExist()
        {
            _httpClient.For($"{MockContentfulApiUrl}&content_type=profile&include=1&fields.slug=nope")
               .Return(HttpResponse.Successful(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetProfile("nope"));
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
            response.Error.Should().Be("No profile found for 'nope'");
        }

    }
}