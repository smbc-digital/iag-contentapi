namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PaymentContentfulFactoryTests
{
    private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private Mock<ITimeProvider> _timeProvider = new();
    private Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();

    [Fact]
    public void ToModel_ShouldCreateAPaymentFromAContentfulPayment()
    {
        // Arrange
        ContentfulPayment contentfulPayment = new ContentfulPaymentBuilder()
            .Slug("payment-slug")
            .Title("payment title")
            .Teaser("payment teaser")
            .ReferenceLabel("reference label")
            .Build();

        PaymentContentfulFactory contentfulFactory = new(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object);

        // Act
        Payment payment = contentfulFactory.ToModel(contentfulPayment);

        // Assert
        Assert.Equal("payment-slug", payment.Slug);
        Assert.Equal("payment title", payment.Title);
        Assert.Equal("payment teaser", payment.Teaser);
        Assert.Equal("reference label", payment.ReferenceLabel);
        Assert.Equal("metaDescription", payment.MetaDescription);
    }
}