using Microsoft.Extensions.Options;
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
    private readonly Mock<ICache> _mockCache;
    private readonly Mock<ILogger<DirectoryRepository>> _mockLogger;
    private readonly Mock<IOptions<RedisExpiryConfiguration>> _mockOptions;

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
            Description = "Directory entry body",
            Teaser = "Directory entry teaser",
            MetaDescription = "Directory entry MetaDescription",
            Name = "Directory entry name",
            Directories = new List<MinimalDirectory>{
                new MinimalDirectory("test-directory", "Test")
            }
        };

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        _mockDirectoryContentfulFactory = new Mock<IContentfulFactory<ContentfulDirectory, Directory>>();
        _mockDirectoryEntryContentfulFactory = new Mock<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>>();
        _mockCache = new Mock<ICache>();
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulDirectory>>>>(), It.IsAny<int>())).ReturnsAsync((IList<ContentfulDirectory>)null);


        _mockLogger = new Mock<ILogger<DirectoryRepository>>();
        _mockOptions = new Mock<IOptions<RedisExpiryConfiguration>>();
        _mockOptions.Setup(options => options.Value).Returns(new RedisExpiryConfiguration {  Directory = 1 });

        _repository  = new DirectoryRepository(config, contentfulClientManager.Object, _mockDirectoryContentfulFactory.Object, _mockDirectoryEntryContentfulFactory.Object, _mockCache.Object, _mockOptions.Object, _mockLogger.Object);
    }

    [Fact]  
    public async void GetDirectoryFromSource_WithSlug_Should_ReturnCorrectType()
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

        var response = await _repository.GetDirectoryFromSource(slug, 2);

        Assert.IsType<Directory>(response);
        Assert.Equal(response, _directory);
    }

    [Fact]  
    public async void GetDirectoryFromSource_WithSlug_Should_ReturnNull_WhenDepthLimitExceeded()
    {
        var response = await _repository.GetDirectoryFromSource("slug", 6);

        Assert.Null(response);
    }

    [Fact]  
    public async void GetDirectoryFromSource_WithSlug_Should_ReturnNull_WhenDirectoryDoesNotExist()
    {
        const string slug = "a-slug";
        var contentfulDirectory = new DirectoryBuilder().WithSlug(slug).Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory>()
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>()});

        _mockDirectoryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectory)).Returns(_directory);

        var response = await _repository.GetDirectoryFromSource("non-existant-slug", 2);

        Assert.Null(response);
    } 

    [Fact]
    public async void Get_WithSlug_Should_GetDirectoryEntries()
    {
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>())).ReturnsAsync( _directory );

        var response = await _repository.Get("directory-slug");

        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<Directory>(), _directory);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()), Times.Once); 
    }

        [Fact]
    public async void Get_WithSlug_Should_Return_FailedNotFound_IfDirectory_DoesNot_Exist()
    {
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>())).ReturnsAsync( (Directory)null);

        var response = await _repository.Get("directory-slug");

        Assert.IsType<HttpResponse>(response);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()), Times.Once); 
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async void GetEntry_WithSlug_Should_ReturnSuccess()
    {
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>())).ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        var response = await _repository.GetEntry("directory-entry-slug");

        Assert.IsType<HttpResponse>(response);
        Assert.Equal(response.Get<DirectoryEntry>(), _directoryEntry);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);         
    }

    [Fact]
    public async void GetEntry_WithSlug_ShouldReturn_NotFound_If_DirectoryEntryDoesNotExist()
    {
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>())).ReturnsAsync(new List<DirectoryEntry> {  });

        var response = await _repository.GetEntry("directory-entry-slug");

        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);         
    }
    

    [Fact]
    public async void GetAllDirectoryEntries_ShouldReturn_IEnumerableOfDirectoryEntry()
    {
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>())).ReturnsAsync(new List<DirectoryEntry> {  });

        var response = await _repository.GetAllDirectoryEntries();

        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once); 
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
    }

    [Fact]
    public async void GetDirectoryEntriesForDirectory_ShouldReturn_CorrectNumberOfDirectories()
    {
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>())).ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        var response = await _repository.GetDirectoryEntriesForDirectory("test-directory");

        Assert.Single(response);
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
    }

    [Fact]
    public async void GetDirectoryEntriesForDirectory_ShouldReturn_Null_IfNoEntriesFound()
    {
        _mockCache.Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>())).ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        var response = await _repository.GetDirectoryEntriesForDirectory("non-existant-test-directory");

        Assert.Empty(response);
    }

[Fact]
    public async void GetAllDirectoryEntriesFromSource()
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

        var response = await _repository.GetAllDirectoryEntriesFromSource();

        // Arrange
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
        Assert.Single(response);
    }
}

