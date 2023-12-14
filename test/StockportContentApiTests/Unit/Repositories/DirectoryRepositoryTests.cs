using Directory = StockportContentApi.Model.Directory;

namespace StockportContentApiTests.Unit.Repositories;

public class DirectoryRepositoryTests
{
    private readonly DirectoryRepository _repository;
    // private readonly Directory _directory;
    
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<DirectoryContentfulFactory> _mockDirectoryContentfulFactory;
    private readonly Mock<DirectoryEntryContentfulFactory> _mockDirectoryEntryContentfulFactory;

    public DirectoryRepositoryTests()
    {
        var config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Build();

        // _directory = new Directory{
        //     Alerts = null,
        //     Slug = "directory-slug",
        //     Body = "Directory body",
        //     CallToAction = null,
        //     BackgroundImage = null,
        //     Teaser = "Directory teaser",
        //     MetaDescription = "Directory MetaDescription",
        //     ContentfulId = "XXX123456",
        //     Title = "Directory title"
        // };

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _mockDirectoryContentfulFactory = new Mock<DirectoryContentfulFactory>();
        _mockDirectoryEntryContentfulFactory = new Mock<DirectoryEntryContentfulFactory>());

        _repository  = new DirectoryRepository(config, contentfulClientManager.Object, _mockDirectoryContentfulFactory.Object, _mockDirectoryEntryContentfulFactory.Object );
    }


    public void Get_WithSlug_Should_ReturnSuccess()
    {
        const string slug = "a-slug";
        var contentfulTopic = new ContentfulTopicBuilder().Slug(slug).Build();
        ContentfulCollection<ContentfulTopic> collection = new()
        {
            Items = new List<ContentfulTopic> { contentfulTopic }
        };

        var builder = new QueryBuilder<ContentfulDirectory>().ContentTypeIs("directory").FieldEquals("fields.slug", "a-slug").Include(1);
        _contentfulClient.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulDirectory>>(q => q.Build() == builder.Build()),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        var response = AsyncTestHelper.Resolve(_repository.Get("a-slug"));

        // Arrange
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<Directory>(), _directory);
    }
}

