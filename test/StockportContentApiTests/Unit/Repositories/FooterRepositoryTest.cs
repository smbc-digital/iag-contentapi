namespace StockportContentApiTests.Unit.Repositories;

public class FooterRepositoryTest
{
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClient> _client;
    private readonly FooterRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulFooter, Footer>> _contentfulFactory;

    public FooterRepositoryTest()
    {
        _config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Build();

        var contentfulClientManager = new Mock<IContentfulClientManager>();

        _client = new Mock<IContentfulClient>();
        _contentfulFactory = new Mock<IContentfulFactory<ContentfulFooter, Footer>>();

        contentfulClientManager.Setup(o => o.GetClient(_config)).Returns(_client.Object);

        _repository = new FooterRepository(_config, contentfulClientManager.Object, _contentfulFactory.Object);
    }

    [Fact]
    public void ShouldReturnAFooter()
    {
        var mockFooter = new Footer("Title", "a-slug", new List<SubItem>(), new List<SocialMediaLink>());

        var footerCollection = new ContentfulCollection<ContentfulFooter>();
        footerCollection.Items = new List<ContentfulFooter>
            {
               new ContentfulFooterBuilder().Build()
            };

        _client.Setup(o => o.GetEntries(
                            It.Is<QueryBuilder<ContentfulFooter>>(q => q.Build() == new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1).Build()),
                            It.IsAny<CancellationToken>())).ReturnsAsync(footerCollection);

        _contentfulFactory.Setup(o => o.ToModel(It.IsAny<ContentfulFooter>()))
            .Returns(new Footer("Title", "a-slug", new List<SubItem>(),
                new List<SocialMediaLink>()));
        var footer = AsyncTestHelper.Resolve(_repository.GetFooter());
        footer.Get<Footer>().Title.Should().Be(mockFooter.Title);
        footer.Get<Footer>().Slug.Should().Be(mockFooter.Slug);
        footer.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
