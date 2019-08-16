using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using StockportContentApi.ContentfulFactories;
using Moq;
using StockportContentApi.Model;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class PaymentContentfulFactoryTest
    {
        private Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;

        [Fact]
        public void ShouldCreateAPaymentFromAContentfulPayment()
        {
            var contentfulPayment = new ContentfulPaymentBuilder()
                .Slug("payment-slug")
                .Title("payment title")
                .Teaser("payment teaser")
                .ReferenceLabel("reference label")
                .Build();

            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            var contentfulFactory = new PaymentContentfulFactory(_crumbFactory.Object, HttpContextFake.GetHttpContextFake());

            var payment = contentfulFactory.ToModel(contentfulPayment);

            payment.Slug.Should().Be("payment-slug");
            payment.Title.Should().Be("payment title");
            payment.Teaser.Should().Be("payment teaser");
            payment.ReferenceLabel.Should().Be("reference label");
            payment.MetaDescription.Should().Be("metaDescription");
        }
    }
}
