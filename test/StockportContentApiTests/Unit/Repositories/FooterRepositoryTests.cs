namespace StockportContentApiTests.Unit.Repositories;

public class FooterRepositoryTests
{
    private readonly ContentfulConfig _config;
    private readonly Mock<IContentfulClient> _client = new();
    private readonly FooterRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulFooter, Footer>> _contentfulFactory = new();

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

        contentfulClientManager.Setup(client=> client.GetClient(_config)).Returns(_client.Object);

        _repository = new(_config, contentfulClientManager.Object, _contentfulFactory.Object);
    }

    [Fact]
    public void ShouldReturnAFooter()
    {
        // Arrange
        Footer mockFooter = new("Title", "a-slug", new List<SubItem>(), new List<SocialMediaLink>(), "footerContent1", "footerContent2", "footerContent3");

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
                new List<SocialMediaLink>(), "footerContent1", "footerContent2", "footerContent3"));

        // Act
        HttpResponse footer = AsyncTestHelper.Resolve(_repository.GetFooter());

        // Assert
        Assert.Equal(mockFooter.Title, footer.Get<Footer>().Title);
        Assert.Equal(mockFooter.Slug, footer.Get<Footer>().Slug);
        footer.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public void Footer_ShouldReturnNotFound_IfFooterIsNull()
    {
        // Arrange
        Footer mockFooter = new("Title", "a-slug", new List<SubItem>(), new List<SocialMediaLink>(), "footerContent1", "footerContent2", "footerContent3");

        ContentfulCollection<ContentfulFooter> footerCollection = new()
        {
            Items = new List<ContentfulFooter>
            {
                new ContentfulFooterBuilder().Build()
            }
        };

        _client
            .Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulFooter>>(
                    q => q.Build().Equals(new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1).Build())),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(footerCollection);

        _contentfulFactory
            .Setup(o => o.ToModel(It.IsAny<ContentfulFooter>()))
            .Returns((Footer)null);

        // Act
        HttpResponse footer = AsyncTestHelper.Resolve(_repository.GetFooter());

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, footer.StatusCode);
    }
}