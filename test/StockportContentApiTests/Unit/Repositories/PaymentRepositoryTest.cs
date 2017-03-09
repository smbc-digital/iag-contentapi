using System;
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
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Xunit;
using IContentfulClient = Contentful.Core.IContentfulClient;
using StockportContentApiTests.Unit.Builders;

namespace StockportContentApiTests.Unit.Repositories
{
    public class PaymentRepositoryTest
    {
        private readonly PaymentRepository _repository;
        private readonly Mock<ITimeProvider> _mockTimeProvider;
        private readonly Mock<IContentfulClient> _contentfulClient;
        private readonly Mock<IHttpClient> _httpClient;
        private readonly Mock<IContentfulFactory<ContentfulPayment, Payment>> _paymentFactory;
        
        public PaymentRepositoryTest()
        {
            var config = new ContentfulConfig("test")
                .Add("DELIVERY_URL", "https://fake.url")
                .Add("TEST_SPACE", "SPACE")
                .Add("TEST_ACCESS_KEY", "KEY")
                .Build();

            var contentfulFactory = new PaymentContentfulFactory();
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
        public void GetsASinglePaymentItemFromASlug()
        {
            // Arrange
            const string slug = "any-payment";
            
            var rawPayment = new ContentfulPaymentBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulPayment>(
                It.Is<QueryBuilder<ContentfulPayment>>(
                     q => q.Build() == builder.Build()),     
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulPayment> {rawPayment});

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));           
            var paymentItem = response.Get<Payment>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            paymentItem.ShouldBeEquivalentTo(rawPayment);            
        }

        [Fact]
        public void ShouldReturn404ForNonExistentSlug()
        {
            // Arrange
            const string slug = "invalid-url";

            var rawPayment = new ContentfulPaymentBuilder().Slug(slug).Build();

            var builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", slug).Include(1);
            _contentfulClient.Setup(o => o.GetEntriesAsync<ContentfulPayment>(
                It.Is<QueryBuilder<ContentfulPayment>>(
                     q => q.Build() == builder.Build()),
                     It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<ContentfulPayment>());

            // Act
            var response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));
            var paymentItem = response.Get<Payment>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

    }
}

