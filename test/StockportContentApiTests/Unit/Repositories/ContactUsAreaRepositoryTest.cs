using System.Net;
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
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;

namespace StockportContentApiTests.Unit.Repositories
{
    public class ContactUsAreaRepositoryTest
    {
        private readonly ContactUsAreaRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _mockSubitemFactory = new();
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new();
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _mockAlertFactory = new();
        private readonly Mock<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>> _mockContactUsCategoryFactory = new();

        private readonly Mock<ITimeProvider> _timeprovider = new Mock<ITimeProvider>();

        public ContactUsAreaRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

            var contentfulFactory = new ContactUsAreaContentfulFactory(_mockSubitemFactory.Object,
                _mockCrumbFactory.Object,
                _timeprovider.Object,
                _mockAlertFactory.Object,
                _mockContactUsCategoryFactory.Object);

            _repository = new ContactUsAreaRepository(config, contentfulClientManager.Object, contentfulFactory);
        }

        [Fact]
        public void ItGetsContactUsArea()
        {
            // Arrange
            const string slug = "contactusarea";

            var collection = new ContentfulCollection<ContentfulContactUsArea>();
            var rawContactUsArea = new ContentfulContactUsAreaBuilder().Slug(slug).Build();
            collection.Items = new List<ContentfulContactUsArea> { rawContactUsArea };

            var builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactUsArea").Include(3);

            _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsArea>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetContactUsArea());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [Fact]
        public void ItReturnsBreadcrumbs()
        {
            // Arrange
            const string slug = "contactusarea";
            var crumb = new Crumb("title", "slug", "type");
            var collection = new ContentfulCollection<ContentfulContactUsArea>();
            var rawContactUsArea = new ContentfulContactUsAreaBuilder().Slug(slug)
                .Breadcrumbs(new List<ContentfulReference>()
                            { new ContentfulReference() {Title = crumb.Title, Slug = crumb.Title, Sys = new SystemProperties() {Type = "Entry" }},
                            })
                .Build();
            collection.Items = new List<ContentfulContactUsArea> { rawContactUsArea };

            var builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactUsArea").Include(3);
            _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulContactUsArea>>(q => q.Build() == builder.Build()), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            _mockCrumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetContactUsArea());

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            var contactUsArea = response.Get<ContactUsArea>();

            contactUsArea.Breadcrumbs.First().Should().Be(crumb);
        }
    }
}
