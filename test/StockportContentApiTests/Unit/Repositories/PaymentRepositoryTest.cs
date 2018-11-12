using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using Contentful.Core.Models;
using Contentful.Core.Search;
using FluentAssertions;
using Moq;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.Repositories
{
    public class PaymentRepositoryTest
    {
        private readonly PaymentRepository _repository;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IHttpClient> _httpClient;
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;

        public PaymentRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Add("TEST_MANAGEMENT_KEY", "KEY")
                .Build();

            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();

            _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
                .Returns(new Crumb("title", "slug", "title"));

            var contentfulFactory = new PaymentContentfulFactory(_crumbFactory.Object, HttpContextFake.GetHttpContextFake());
            _httpClient = new Mock<IHttpClient>();
            
            var contentfulClientManager = new Mock<IContentfulClientManager>();
            _contentfulClient = new Mock<IContentfulClient>();
            contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
            _repository = new PaymentRepository
            (
                config, 
                contentfulClientManager.Object,
                contentfulFactory
            );
        }

        [Fact]
        public void ShouldGetAllPayments()
        {
            // Arrange          
            var rawPayments = new List<ContentfulPayment>();
            rawPayments.Add(new ContentfulPaymentBuilder().Slug("firstPayment").Build());
            rawPayments.Add(new ContentfulPaymentBuilder().Slug("secondPayment").Build());
            rawPayments.Add(new ContentfulPaymentBuilder().Slug("thirdPayment").Build());
            var collection = new ContentfulCollection<ContentfulPayment>();
            collection.Items = rawPayments;

            var builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);
            _contentfulClient.Setup(o => o.GetEntries(
                It.Is<QueryBuilder<ContentfulPayment>>(
                     q => q.Build() == builder.Build()),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            // Act
             var response = AsyncTestHelper.Resolve(_repository.Get());
            var payments = response.Get<List<Payment>>();          

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            payments.Count.Should().Be(rawPayments.Count);
            payments[0].Slug.Should().Be(rawPayments[0].Slug);
            payments[1].Slug.Should().Be(rawPayments[1].Slug);
            payments[2].Slug.Should().Be(rawPayments[2].Slug);
        }

        [Fact]
        public void ShouldGetsASinglePaymentItemFromASlug()
        {
            // Arrange
            const string slug = "any-payment";
            
            var rawPayment = new ContentfulPaymentBuilder().Slug(slug).Build();
            var collection = new ContentfulCollection<ContentfulPayment>();
            collection.Items = new List<ContentfulPayment> { rawPayment };

            var builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntries(
                It.Is<QueryBuilder<ContentfulPayment>>(
                     q => q.Build() == builder.Build()),     
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));           
            var paymentItem = response.Get<Payment>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            paymentItem.Description.Should().Be(rawPayment.Description);
            paymentItem.Title.Should().Be(rawPayment.Title);
            paymentItem.Teaser.Should().Be(rawPayment.Teaser);
            paymentItem.Slug.Should().Be(rawPayment.Slug);
            paymentItem.PaymentDetailsText.Should().Be(rawPayment.PaymentDetailsText);
            paymentItem.ParisReference.Should().Be(rawPayment.ParisReference);
            paymentItem.Breadcrumbs.First().Title.Should().Be("title");
        }

        [Fact]
        public void ShouldReturn404ForNonExistentSlug()
        {
            // Arrange
            const string slug = "invalid-url";

            var collection = new ContentfulCollection<ContentfulPayment>();
            collection.Items = new List<ContentfulPayment>();

            var builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntries(
                It.IsAny<QueryBuilder<ContentfulPayment>>(),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));
            
            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}

