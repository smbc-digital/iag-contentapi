namespace StockportContentApiTests.Unit.Repositories;

public class FooterRepositoryTests
{
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClient> _client;
    private readonly FooterRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulFooter, Footer>> _contentfulFactory;

    public FooterRepositoryTests()
    {
        _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        Mock<IContentfulClientManager> contentfulClientManager = new();

        _client = new Mock<IContentfulClient>();
        _contentfulFactory = new Mock<IContentfulFactory<ContentfulFooter, Footer>>();

        contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);

        _repository = new FooterRepository(_config, contentfulClientManager.Object, _contentfulFactory.Object);
    }

    [Fact]
    public void ShouldReturnAFooter()
    {
        Footer mockFooter = new("Title", "a-slug", new List<SubItem>(), new List<SocialMediaLink>());

        ContentfulCollection<ContentfulFooter> footerCollection = new()
        {
            Items = new List<ContentfulFooter>
            {
               new ContentfulFooterBuilder().Build()
            }
        };

        _client.Setup(o => o.GetEntries(
                            It.Is<QueryBuilder<ContentfulFooter>>(q => q.Build().Equals(new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1).Build())),
                            It.IsAny<CancellationToken>())).ReturnsAsync(footerCollection);

        _contentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulFooter>()))
            .Returns(new Footer("Title", "a-slug", new List<SubItem>(),
                new List<SocialMediaLink>()));
        HttpResponse footer = AsyncTestHelper.Resolve(_repository.GetFooter());
        footer.Get<Footer>().Title.Should().Be(mockFooter.Title);
        footer.Get<Footer>().Slug.Should().Be(mockFooter.Slug);
        footer.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
