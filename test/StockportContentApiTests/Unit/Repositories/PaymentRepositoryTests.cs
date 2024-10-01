namespace StockportContentApiTests.Unit.Repositories;

public class PaymentRepositoryTests
{
    private readonly PaymentRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;
    private readonly Mock<ITimeProvider> _timeProvider;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;

    public PaymentRepositoryTests()
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

        PaymentContentfulFactory contentfulFactory = new(_alertFactory.Object, _timeProvider.Object, _crumbFactory.Object);

        Mock<IContentfulClientManager> contentfulClientManager = new();
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
    public void ShouldGetAllPayments()
    {
        // Arrange          
        List<ContentfulPayment> rawPayments = new();
        rawPayments.Add(new ContentfulPaymentBuilder().Slug("firstPayment").Build());
        rawPayments.Add(new ContentfulPaymentBuilder().Slug("secondPayment").Build());
        rawPayments.Add(new ContentfulPaymentBuilder().Slug("thirdPayment").Build());
        ContentfulCollection<ContentfulPayment> collection = new();
        collection.Items = rawPayments;

        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);
        _contentfulClient.Setup(o => o.GetEntries(
            It.Is<QueryBuilder<ContentfulPayment>>(
                 q => q.Build() == builder.Build()),
                 It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get());
        List<Payment> payments = response.Get<List<Payment>>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        payments.Count.Should().Be(rawPayments.Count);
        payments[0].Slug.Should().Be(rawPayments[0].Slug);
        payments[1].Slug.Should().Be(rawPayments[1].Slug);
        payments[2].Slug.Should().Be(rawPayments[2].Slug);
    }

    [Fact]
    public void ShouldGetsASinglePaymentItemFromASlug()
    {
        // Arrange
        const string slug = "any-payment";

        ContentfulPayment rawPayment = new ContentfulPaymentBuilder().Slug(slug).Build();
        ContentfulCollection<ContentfulPayment> collection = new();
        collection.Items = new List<ContentfulPayment> { rawPayment };

        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", slug).Include(1);
        _contentfulClient.Setup(o => o.GetEntries(
            It.Is<QueryBuilder<ContentfulPayment>>(
                 q => q.Build() == builder.Build()),
                 It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));
        Payment paymentItem = response.Get<Payment>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        paymentItem.Description.Should().Be(rawPayment.Description);
        paymentItem.Title.Should().Be(rawPayment.Title);
        paymentItem.Teaser.Should().Be(rawPayment.Teaser);
        paymentItem.Slug.Should().Be(rawPayment.Slug);
        paymentItem.PaymentDetailsText.Should().Be(rawPayment.PaymentDetailsText);
        paymentItem.ParisReference.Should().Be(rawPayment.ParisReference);
        paymentItem.Breadcrumbs.First().Title.Should().Be("title");
    }

    [Fact]
    public void ShouldReturn404ForNonExistentSlug()
    {
        // Arrange
        const string slug = "invalid-url";

        ContentfulCollection<ContentfulPayment> collection = new();
        collection.Items = new List<ContentfulPayment>();

        QueryBuilder<ContentfulPayment> builder = new QueryBuilder<ContentfulPayment>().ContentTypeIs("payment").FieldEquals("fields.slug", slug).Include(1);
        _contentfulClient.Setup(o => o.GetEntries(
            It.IsAny<QueryBuilder<ContentfulPayment>>(),
                 It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetPayment(slug));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

}

