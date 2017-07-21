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
    public class StartPageRepositoryTest : TestingBaseClass
    {
        private readonly FakeHttpClient _httpClient = new FakeHttpClient();
        private readonly StartPageRepository _repository;
        private const string MockContentfulApiUrl = "https://fake.url/spaces/SPACE/entries?access_token=KEY";
        private readonly List<Alert> _alerts = new List<Alert>() { new Alert("title", "subHeading", "body", "severity", new DateTime(), new DateTime()) };

        public StartPageRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            var mockBreadcrumbFactory = new Mock<IBuildContentTypesFromReferences<Crumb>>();
            var mockalertFactory = new Mock<IBuildContentTypesFromReferences<Alert>>();
            mockalertFactory.Setup(o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(_alerts);
            mockBreadcrumbFactory.Setup( o => o.BuildFromReferences(It.IsAny<IEnumerable<dynamic>>(), It.IsAny<ContentfulResponse>()))
                .Returns(new List<Crumb>() {new NullCrumb()});

            _repository = new StartPageRepository(config, _httpClient, new StartPageFactory(mockBreadcrumbFactory.Object, mockalertFactory.Object));
        }

        [Fact]
        public void GivenThereIsItemInTheContentResponse_ItReturnsOKResponseWithTheContentOfStartPage()
        {
            _httpClient.For($"{MockContentfulApiUrl}&content_type=startPage&include=1&fields.slug=start-page")
                .Return(HttpResponse.Successful(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.StartPage.json")));

            var startPageSlug = "start-page";
            var response = AsyncTestHelper.Resolve(_repository.GetStartPage(startPageSlug));
            var startPage = response.Get<StartPage>();

            response.StatusCode.Should().Be(HttpStatusCode.OK);
            startPage.Title.Should().Be("Start Page");
            startPage.Slug.Should().Be(startPageSlug);
            startPage.Teaser.Should().Be("this is a teaser");
            startPage.Summary.Should().Be("This is a summary ");
            startPage.UpperBody.Should().Be("An upper body");
            startPage.FormLink.Should().Be("http://start.com");
            startPage.FormLinkLabel.Should().Be("Start now");
            startPage.LowerBody.Should().Be("Lower body");
            startPage.BackgroundImage.Should().Be("image.jpg");
            startPage.Icon.Should().Be("icon");
            startPage.Breadcrumbs.Should().HaveCount(1);
            startPage.Alerts.Should().BeEquivalentTo(_alerts);
        }

        [Fact]
        public void GivenNoItemsInTheContentResponse_ItReturnsNotFoundResponse()
        {
            _httpClient.For($"{MockContentfulApiUrl}&content_type=startPage&include=1&fields.slug=new-start-page")
                .Return(HttpResponse.Successful(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.ContentNotFound.json")));

            var response = AsyncTestHelper.Resolve(_repository.GetStartPage("new-start-page"));

            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}
