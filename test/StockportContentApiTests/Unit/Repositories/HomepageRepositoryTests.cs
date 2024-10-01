namespace StockportContentApiTests.Unit.Repositories;

public class HomepageRepositoryTests
{
    private readonly HomepageRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulHomepage, Homepage>> _homepageFactory;
    private readonly Mock<IContentfulClient> _client;

    public HomepageRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _homepageFactory = new Mock<IContentfulFactory<ContentfulHomepage, Homepage>>();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _client = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_client.Object);

        _repository = new HomepageRepository(config, contentfulClientManager.Object, _homepageFactory.Object);
    }

    [Fact]
    public void ItGetsHomepage()
    {
        ContentfulHomepage contentfulHomepage = new();
        ContentfulCollection<ContentfulHomepage> collection = new()
        {
            Items = new List<ContentfulHomepage> { contentfulHomepage }
        };

        QueryBuilder<ContentfulHomepage> builder = new QueryBuilder<ContentfulHomepage>().ContentTypeIs("homepage").Include(2);
        _client.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulHomepage>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _homepageFactory.Setup(o => o.ToModel(It.IsAny<ContentfulHomepage>()))
            .Returns(new Homepage(new List<string>(), string.Empty, string.Empty, new List<SubItem>(), new List<SubItem>(), new List<Alert>(), new List<CarouselContent>(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty, string.Empty, new CarouselContent(), new CallToActionBanner(), new CallToActionBanner(), null));

        HttpResponse response = AsyncTestHelper.Resolve(_repository.Get());
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
