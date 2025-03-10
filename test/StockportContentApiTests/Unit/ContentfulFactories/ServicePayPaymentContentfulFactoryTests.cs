namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ServicePayPaymentContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();

    [Fact]
    public void ShouldCreateAServicePayPaymentFromAContentfulServicePayPayment()
    {
        // Arrange
        ContentfulServicePayPayment contentfulPayment = new ContentfulServicePayPaymentBuilder()
            .Slug("payment-slug")
            .Title("payment title")
            .Teaser("payment teaser")
            .ReferenceLabel("reference label")
            .PaymentAmount("15.23")
            .Build();

        ServicePayPaymentContentfulFactory contentfulFactory = new(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object);

        // Act
        ServicePayPayment payment = contentfulFactory.ToModel(contentfulPayment);

        // Assert
        Assert.Equal("payment-slug", payment.Slug);
        Assert.Equal("payment title", payment.Title);
        Assert.Equal("payment teaser", payment.Teaser);
        Assert.Equal("reference label", payment.ReferenceLabel);
        Assert.Equal("metaDescription", payment.MetaDescription);
        Assert.Equal("15.23", payment.PaymentAmount);
    }
}