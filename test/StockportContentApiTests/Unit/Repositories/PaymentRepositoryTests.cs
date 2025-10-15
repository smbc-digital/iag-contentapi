using System.Threading.Tasks;

namespace StockportContentApiTests.Unit.Repositories;

public class PaymentRepositoryTests
{
    private readonly PaymentRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();

    public PaymentRepositoryTests()
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
            .Returns(new Crumb("title", "slug", "title", new List<string>()));

        PaymentContentfulFactory contentfulFactory = new(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(client => client.GetClient(config))
            .Returns(_contentfulClient.Object);
        
        _repository = new PaymentRepository(config, contentfulClientManager.Object, contentfulFactory);
    }

    [Fact]
    public async Task Get_ShouldReturnAllPayments()
    {
        // Arrange          
        List<ContentfulPayment> rawPayments = new()
        {
            new ContentfulPaymentBuilder().Slug("firstPayment").Build(),
            new ContentfulPaymentBuilder().Slug("secondPayment").Build(),
            new ContentfulPaymentBuilder().Slug("thirdPayment").Build()
        };

        ContentfulCollection<ContentfulPayment> collection = new()
        {
            Items = rawPayments
        };

        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);
        _contentfulClient
            .Setup(client => client.GetEntries(It.Is<QueryBuilder<ContentfulPayment>>(query => query.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.Get();
        List<Payment> payments = response.Get<List<Payment>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(rawPayments.Count, payments.Count);
        Assert.Equal(rawPayments[0].Slug, payments[0].Slug);
        Assert.Equal(rawPayments[1].Slug, payments[1].Slug);
        Assert.Equal(rawPayments[2].Slug, payments[2].Slug);
    }

    [Fact]
    public async Task GetPayment_ShouldGetsASinglePaymentItemFromASlug()
    {
        // Arrange
        ContentfulPayment rawPayment = new ContentfulPaymentBuilder().Slug("any-payment").Build();
        ContentfulCollection<ContentfulPayment> collection = new()
        {
            Items = new List<ContentfulPayment> { rawPayment }
        };

        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", "any-payment").Include(1);
        _contentfulClient
            .Setup(client => client.GetEntries(It.Is<QueryBuilder<ContentfulPayment>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.GetPayment("any-payment");
        Payment paymentItem = response.Get<Payment>();

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
        ContentfulCollection<ContentfulPayment> collection = new()
        {
            Items = new List<ContentfulPayment>()
        };

        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", "slug").Include(1);
        _contentfulClient
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulPayment>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.GetPayment("invalid-url");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}