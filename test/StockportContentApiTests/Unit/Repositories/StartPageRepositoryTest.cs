using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
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
using StockportContentApiTests.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.Repositories
{
    public class StartPageRepositoryTest : TestingBaseClass
    {
        private readonly Mock<IContentfulFactory<ContentfulStartPage, StartPage>> _startPageFactory;
        private readonly Mock<IContentfulClient> _client;
        private readonly StartPageRepository _repository;

        public StartPageRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _client = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

            _startPageFactory = new Mock<IContentfulFactory<ContentfulStartPage, StartPage>>();

            _repository = new StartPageRepository(config, contentfulClientManager.Object, _startPageFactory.Object);
        }

        [Fact]
        public void GivenThereIsItemInTheContentResponse_ItReturnsOKResponseWithTheContentOfStartPage()
        {
            // Arrange
            string slug = "startpage_slug";
            var ContentfulStartPage = new ContentfulStartPageBuilder().Slug(slug).Build();
            var collection = new ContentfulCollection<ContentfulStartPage>();
            collection.Items = new List<ContentfulStartPage> { ContentfulStartPage };

            List<Alert> _alerts = new List<Alert> { new Alert("title", "subHeading", "body",
                "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), string.Empty, false) };

            List<Alert> _inlineAlerts = new List<Alert> { new Alert("title", "subHeading", "body",
                "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), string.Empty, false) };

            var startPageItem = new StartPage("Start Page", "startPageSlug", "this is a teaser", "This is a summary", "An upper body", "Start now", "http://start.com", "Lower body", "image.jpg","icon", new List<Crumb> { new Crumb("title", "slug", "type") }, _alerts, _inlineAlerts);

            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("startPage").FieldEquals("fields.slug", slug).Include(3);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulStartPage>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);

            _startPageFactory.Setup(o => o.ToModel(It.IsAny<ContentfulStartPage>())).Returns(startPageItem);

            var response = AsyncTestHelper.Resolve(_repository.GetStartPage(slug));

            var startPage = response.Get<StartPage>();

            // Act

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            startPage.Title.Should().Be("Start Page");
            startPage.Slug.Should().Be("startPageSlug");
            startPage.Teaser.Should().Be("this is a teaser");
            startPage.Summary.Should().Be("This is a summary");
            startPage.UpperBody.Should().Be("An upper body");
            startPage.FormLink.Should().Be("http://start.com");
            startPage.FormLinkLabel.Should().Be("Start now");
            startPage.LowerBody.Should().Be("Lower body");
            startPage.BackgroundImage.Should().Be("image.jpg");
            startPage.Icon.Should().Be("icon");
            startPage.Breadcrumbs.Should().HaveCount(1);
            startPage.Alerts.Should().BeEquivalentTo(_alerts);
            startPage.AlertsInline.Should().BeEquivalentTo(_inlineAlerts);
        }

        [Fact]
        public void GivenNoItemsInTheContentResponse_ItReturnsNotFoundResponse()
        {
            // Arrange
            string slug = "startpage_slug";
           
            var collection = new ContentfulCollection<ContentfulStartPage>();
            collection.Items = new List<ContentfulStartPage>();

            var builder = new QueryBuilder<ContentfulRedirect>().ContentTypeIs("startPage").FieldEquals("fields.slug", slug).Include(3);

            _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulStartPage>>(q => q.Build() == builder.Build()),
                It.IsAny<CancellationToken>())).ReturnsAsync(collection);
            
            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetStartPage(slug));

            //Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}
