using Directory = StockportContentApi.Model.Directory;

namespace StockportContentApiTests.Unit.Repositories;

public class DirectoryRepositoryTests
{
    private readonly DirectoryRepository _repository;
    private readonly Directory _directory;
    private readonly DirectoryEntry _directoryEntry;
    private readonly Mock<IContentfulClient> _contentfulClient;
    private readonly Mock<IContentfulFactory<ContentfulDirectory, Directory>> _mockDirectoryContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>> _mockDirectoryEntryContentfulFactory;

    public DirectoryRepositoryTests()
    {
        var config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Build();

        _directory = new Directory{
            Alerts = null,
            Slug = "directory-slug",
            Body = "Directory body",
            CallToAction = null,
            BackgroundImage = null,
            Teaser = "Directory teaser",
            MetaDescription = "Directory MetaDescription",
            ContentfulId = "XXX123456",
            Title = "Directory title"
        };

        _directoryEntry = new DirectoryEntry{
            Alerts = null,
            Slug = "directory-entry-slug",
            Body = "Directory entry body",
            Teaser = "Directory entry teaser",
            MetaDescription = "Directory entry MetaDescription",
            Title = "Directory entry title"
        };

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _mockDirectoryContentfulFactory = new Mock<IContentfulFactory<ContentfulDirectory, Directory>>();
        _mockDirectoryEntryContentfulFactory = new Mock<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>>();

        _repository  = new DirectoryRepository(config, contentfulClientManager.Object, _mockDirectoryContentfulFactory.Object, _mockDirectoryEntryContentfulFactory.Object );
    }

    [Fact]
    public async void Get_WithSlug_Should_ReturnSuccess()
    {
        const string slug = "a-slug";
        var contentfulDirectory = new DirectoryBuilder().WithSlug(slug).Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory> { contentfulDirectory }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>()});

        _mockDirectoryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectory)).Returns(_directory);

        var response = await _repository.Get(slug);

        // Arrange
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<Directory>(), _directory);
    }

        [Fact]
    public async void Get_WithSlug_Should_GetDirecotryEntries()
    {
        const string slug = "a-slug";
        var contentfulDirectory = new DirectoryBuilder().WithSlug(slug).Build();
        var contentfulDirectoryEntry = new DirectoryEntryBuilder().WithSlug(slug).Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory> { contentfulDirectory }
        };

        ContentfulCollection<ContentfulDirectoryEntry> entryCollection = new()
        {
            Items = new List<ContentfulDirectoryEntry> { contentfulDirectoryEntry }
        };
        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(entryCollection);

        _mockDirectoryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectory)).Returns(_directory);

        var response = await _repository.Get(slug);

        // Arrange
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _contentfulClient.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async void Get_WithSlug_Should_Return_NotFound_If_DirectoryDoesNotExist()
    {
        const string slug = "a-slug";
        var contentfulDirectory = new DirectoryBuilder().WithSlug(slug).Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory>()
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _mockDirectoryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectory)).Returns(_directory);

        var response = await _repository.Get(slug);

        // Arrange
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

        [Fact]
    public async void GetEntry_WithSlug_Should_ReturnSuccess()
    {
        const string slug = "a-slug";
        var contentfulDirectoryEntry = new DirectoryEntryBuilder().WithSlug(slug).Build();
        ContentfulCollection<ContentfulDirectoryEntry> collection = new()
        {
            Items = new List<ContentfulDirectoryEntry> { contentfulDirectoryEntry }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _mockDirectoryEntryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectoryEntry)).Returns(_directoryEntry);

        var response = await _repository.GetEntry(slug);

        // Arrange
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<DirectoryEntry>(), _directoryEntry);
    }

        [Fact]
    public async void GetEntry_WithSlug_Should_Return_NotFound_If_DirectoryEntryDoesNotExist()
    {
        const string slug = "a-slug";
        var contentfulDirectoryEntry = new DirectoryEntryBuilder().WithSlug(slug).Build();
        ContentfulCollection<ContentfulDirectoryEntry> collection = new()
        {
            Items = new List<ContentfulDirectoryEntry>()
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _mockDirectoryEntryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectoryEntry)).Returns(_directoryEntry);

        var response = await _repository.GetEntry(slug);

        // Arrange
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}

