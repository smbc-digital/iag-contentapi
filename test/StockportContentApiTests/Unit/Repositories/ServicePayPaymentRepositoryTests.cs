namespace StockportContentApiTests.Unit.Repositories;

public class ServicePayPaymentRepositoryTests
{
    private readonly ServicePayPaymentRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<ITimeProvider> _timeProvider;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;

    public ServicePayPaymentRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _timeProvider = new Mock<ITimeProvider>();
        _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();

        _crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new Crumb("title", "slug", "title"));

        ServicePayPaymentContentfulFactory contentfulFactory = new(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object);

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);
        _repository = new ServicePayPaymentRepository
        (
            config,
            contentfulClientManager.Object,
            contentfulFactory
        );
    }

    [Fact]
    public void ShouldGetsASinglePaymentItemFromASlug()
    {
        // Arrange
        const string slug = "any-payment";

        ContentfulServicePayPayment rawPayment = new ContentfulServicePayPaymentBuilder().Slug(slug).AccountReference("accountRef").CatalogueId("catId").Build();
        ContentfulCollection<ContentfulServicePayPayment> collection = new()
        {
            Items = new List<ContentfulServicePayPayment> { rawPayment }
        };

        QueryBuilder<ContentfulServicePayPayment> builder = new QueryBuilder<ContentfulServicePayPayment>().ContentTypeIs("servicePayPayment").FieldEquals("fields.slug", slug).Include(1);

        _contentfulClient.Setup(o => o.GetEntries(
            It.Is<QueryBuilder<ContentfulServicePayPayment>>(
                 q => q.Build() == builder.Build()),
                 It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));
        ServicePayPayment paymentItem = response.Get<ServicePayPayment>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentItem.Description.Should().Be(rawPayment.Description);
        paymentItem.Title.Should().Be(rawPayment.Title);
        paymentItem.Teaser.Should().Be(rawPayment.Teaser);
        paymentItem.Slug.Should().Be(rawPayment.Slug);
        paymentItem.PaymentDetailsText.Should().Be(rawPayment.PaymentDetailsText);
        paymentItem.Breadcrumbs.First().Title.Should().Be("title");
    }

    [Fact]
    public void ShouldReturn404ForNonExistentSlug()
    {
        // Arrange
        const string slug = "invalid-url";

        ContentfulCollection<ContentfulServicePayPayment> collection = new()
        {
            Items = new List<ContentfulServicePayPayment>()
        };

        _contentfulClient.Setup(o => o.GetEntries(
            It.IsAny<QueryBuilder<ContentfulServicePayPayment>>(),
                 It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}