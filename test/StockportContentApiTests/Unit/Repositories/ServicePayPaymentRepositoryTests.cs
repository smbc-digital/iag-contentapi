using System.Threading.Tasks;

namespace StockportContentApiTests.Unit.Repositories;

public class ServicePayPaymentRepositoryTests
{
    private readonly ServicePayPaymentRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();

    public ServicePayPaymentRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _crumbFactory
            .Setup(crumb => crumb.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new Crumb("title", "slug", "title"));

        ServicePayPaymentContentfulFactory contentfulFactory = new(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(client => client.GetClient(config))
            .Returns(_contentfulClient.Object);
        
        _repository = new ServicePayPaymentRepository(config, contentfulClientManager.Object, contentfulFactory);
    }

    [Fact]
    public async Task ShouldGetsASinglePaymentItemFromASlug()
    {
        // Arrange
        ContentfulServicePayPayment rawPayment = new ContentfulServicePayPaymentBuilder()
            .Slug("any-payment")
            .AccountReference("accountRef")
            .CatalogueId("catId")
            .Build();

        ContentfulCollection<ContentfulServicePayPayment> collection = new()
        {
            Items = new List<ContentfulServicePayPayment> { rawPayment }
        };


        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulServicePayPayment>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.GetPayment("any-payment", "tagId");
        ServicePayPayment paymentItem = response.Get<ServicePayPayment>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(rawPayment.Description, paymentItem.Description);
        Assert.Equal(rawPayment.Title, paymentItem.Title);
        Assert.Equal(rawPayment.Teaser, paymentItem.Teaser);
        Assert.Equal(rawPayment.Slug, paymentItem.Slug);
        Assert.Equal(rawPayment.PaymentDetailsText, paymentItem.PaymentDetailsText);
        Assert.Equal("title", paymentItem.Breadcrumbs.First().Title);
    }

    [Fact]
    public async Task GetPayment_ShouldReturn404ForNonExistentSlug()
    {
        // Arrange
        ContentfulCollection<ContentfulServicePayPayment> collection = new()
        {
            Items = new List<ContentfulServicePayPayment>()
        };

        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulServicePayPayment>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.GetPayment("invalid-url", "tagId");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}