namespace StockportContentApiTests.Unit.Repositories;

public class FooterRepositoryTests
{
    private readonly ContentfulConfig _config;
    private readonly FooterRepository _repository;
    private readonly Mock<IContentfulClient> _client = new();
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

        contentfulClientManager
            .Setup(client => client.GetClient(_config))
            .Returns(_client.Object);

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

        _client
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulFooter>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(footerCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(It.IsAny<ContentfulFooter>()))
            .Returns(new Footer("Title", "a-slug", new List<SubItem>(), new List<SocialMediaLink>(), "footerContent1", "footerContent2", "footerContent3"));

        // Act
        HttpResponse footer = AsyncTestHelper.Resolve(_repository.GetFooter("tagId"));

        // Assert
        Assert.Equal(mockFooter.Title, footer.Get<Footer>().Title);
        Assert.Equal(mockFooter.Slug, footer.Get<Footer>().Slug);
        Assert.Equal(HttpStatusCode.OK, footer.StatusCode);
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
            .Setup(client => client.GetEntries(It.IsAny<QueryBuilder<ContentfulFooter>>(),
                                            It.IsAny<CancellationToken>()))
            .ReturnsAsync(footerCollection);

        _contentfulFactory
            .Setup(contentfulFactory => contentfulFactory.ToModel(It.IsAny<ContentfulFooter>()))
            .Returns((Footer)null);

        // Act
        HttpResponse footer = AsyncTestHelper.Resolve(_repository.GetFooter("tagId"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, footer.StatusCode);
    }
}