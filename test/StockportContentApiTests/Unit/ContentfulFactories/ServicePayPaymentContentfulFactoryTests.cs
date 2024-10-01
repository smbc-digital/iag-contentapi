namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ServicePayPaymentContentfulFactoryTests
{
    private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private Mock<ITimeProvider> _timeProvider;
    private Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;

    [Fact]
    public void ShouldCreateAServicePayPaymentFromAContentfulServicePayPayment()
    {
        ContentfulServicePayPayment contentfulPayment = new ContentfulServicePayPaymentBuilder()
            .Slug("payment-slug")
            .Title("payment title")
            .Teaser("payment teaser")
            .ReferenceLabel("reference label")
            .PaymentAmount("15.23")
            .Build();

        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _timeProvider = new Mock<ITimeProvider>();
        _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
        ServicePayPaymentContentfulFactory contentfulFactory = new(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object);

        ServicePayPayment payment = contentfulFactory.ToModel(contentfulPayment);

        payment.Slug.Should().Be("payment-slug");
        payment.Title.Should().Be("payment title");
        payment.Teaser.Should().Be("payment teaser");
        payment.ReferenceLabel.Should().Be("reference label");
        payment.MetaDescription.Should().Be("metaDescription");
        payment.PaymentAmount.Should().Be("15.23");
    }
}