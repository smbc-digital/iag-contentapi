using Xunit;
using FluentAssertions;
using StockportContentApiTests.Unit.Builders;
using StockportContentApi.ContentfulModels;
using StockportContentApi.ContentfulFactories;
using Moq;
using StockportContentApi.Model;
using StockportContentApi.Fakes;
using StockportContentApi.Utils;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class PaymentContentfulFactoryTest
    {
        private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
        private Mock<ITimeProvider> _timeProvider;
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

            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _timeProvider = new Mock<ITimeProvider>();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            var contentfulFactory = new PaymentContentfulFactory(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object, HttpContextFake.GetHttpContextFake());

            var payment = contentfulFactory.ToModel(contentfulPayment);

            payment.Slug.Should().Be("payment-slug");
            payment.Title.Should().Be("payment title");
            payment.Teaser.Should().Be("payment teaser");
            payment.ReferenceLabel.Should().Be("reference label");
            payment.MetaDescription.Should().Be("metaDescription");
        }
    }
}
