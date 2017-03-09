using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using Contentful.Core.Models;
using StockportContentApi.ContentfulFactories;
using StockportContentApiTests.Unit.Repositories;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class PaymentContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAPaymentFromAContentfulPayment()
        {
            var contentfulPayment = new ContentfulPaymentBuilder()
                .Slug("payment-slug")
                .Title("payment title")
                .ReferenceLabel("reference label")
                .Build();

            var contentfulFactory = new PaymentContentfulFactory();

            var payment = contentfulFactory.ToModel(contentfulPayment);

            payment.Slug.Should().Be("payment-slug");
            payment.Title.Should().Be("payment title");
            payment.ReferenceLabel.Should().Be("reference label");
        }
    }
}
