using Microsoft.Extensions.Options;
using Directory = StockportContentApi.Models.Directory;

namespace StockportContentApiTests.Unit.Repositories;

public class DirectoryRepositoryTests
{
    private readonly DirectoryRepository _repository;
    private readonly Directory _directory;
    private readonly DirectoryEntry _directoryEntry;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulDirectory, Directory>> _mockDirectoryContentfulFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>> _mockDirectoryEntryContentfulFactory = new();
    private readonly Mock<ICache> _mockCache = new();
    private readonly Mock<IOptions<RedisExpiryConfiguration>> _mockOptions = new();

    public DirectoryRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _directory = new Directory
        {
            Alerts = null,
            Slug = "directory-slug",
            Body = "Directory body",
            CallToAction = null,
            BackgroundImage = null,
            Teaser = "Directory teaser",
            MetaDescription = "Directory MetaDescription",
            ContentfulId = "XXX123456",
            Title = "Directory title",
            PinnedEntries = null
        };

        _directoryEntry = new DirectoryEntry
        {
            Alerts = null,
            Slug = "directory-entry-slug",
            Description = "Directory entry body",
            Teaser = "Directory entry teaser",
            MetaDescription = "Directory entry MetaDescription",
            Name = "Directory entry name",
            Directories = new List<MinimalDirectory>{
                new("test-directory", "Test")
            }
        };

        Mock<IContentfulClientManager> contentfulClientManager = new();
        contentfulClientManager
            .Setup(_ => _.GetClient(config))
            .Returns(_contentfulClient.Object);

        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulDirectory>>>>(), It.IsAny<int>()))
            .ReturnsAsync((IList<ContentfulDirectory>)null);

        _mockOptions
            .Setup(options => options.Value)
            .Returns(new RedisExpiryConfiguration { Directory = 1 });

        _repository = new DirectoryRepository(config,
                                            contentfulClientManager.Object,
                                            _mockDirectoryContentfulFactory.Object,
                                            _mockDirectoryEntryContentfulFactory.Object,
                                            _mockCache.Object,
                                            _mockOptions.Object);
}

    [Fact]
    public async Task GetDirectoryFromSource_WithSlug_Should_ReturnCorrectType()
    {
        // Arrange
        const string slug = "a-slug";
        ContentfulDirectory contentfulDirectory = new DirectoryBuilder().WithSlug(slug).Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory> { contentfulDirectory }
        };

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        _contentfulClient.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(),
            It.IsAny<CancellationToken>())).ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>() });

        _mockDirectoryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectory)).Returns(_directory);

        // Act
        Directory response = await _repository.GetDirectoryFromSource(slug, 2);

        // Assert
        Assert.IsType<Directory>(response);
        Assert.Equal(response, _directory);
    }

    [Fact]
    public async Task GetDirectoryFromSource_WithSlug_Should_ReturnNull_WhenDepthLimitExceeded()
    {
        // Act
        Directory response = await _repository.GetDirectoryFromSource("slug", 6);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task GetDirectoryFromSource_WithSlug_Should_ReturnNull_WhenDirectoryDoesNotExist()
    {
        // Arrange
        ContentfulDirectory contentfulDirectory = new DirectoryBuilder().WithSlug("a-slug").Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory>()
        };

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>() });

        _mockDirectoryContentfulFactory
            .Setup(_ => _.ToModel(contentfulDirectory))
            .Returns(_directory);

        // Act
        Directory response = await _repository.GetDirectoryFromSource("non-existant-slug", 2);

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task Get_WithSlug_Should_GetDirectoryEntries()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()))
            .ReturnsAsync(_directory);

        // Act
        HttpResponse response = await _repository.Get("directory-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<Directory>(), _directory);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Get_WithSlug_Should_Return_FailedNotFound_IfDirectory_DoesNot_Exist()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()))
            .ReturnsAsync((Directory)null);

        // Act
        HttpResponse response = await _repository.Get("directory-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()), Times.Once);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_Return_FailedNotFound_IfNoEntriesExist()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()))
            .ReturnsAsync((Directory)null);

        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_GetDirectoryEntries()
    {
        // Arrange
        ContentfulDirectory contentfulDirectory = new DirectoryBuilder().WithSlug("a-slug").Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory> { contentfulDirectory }
        };

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>() });

        _mockDirectoryContentfulFactory
            .Setup(_ => _.ToModel(contentfulDirectory))
            .Returns(_directory);

        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<List<Directory>>(), new List<Directory>() { _directory });
    }

    [Fact]
    public async Task GetEntry_WithSlug_Should_ReturnSuccess()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        // Act
        HttpResponse response = await _repository.GetEntry("directory-entry-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(response.Get<DirectoryEntry>(), _directoryEntry);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetEntry_WithSlug_ShouldReturn_NotFound_If_DirectoryEntryDoesNotExist()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { });

        // Act
        HttpResponse response = await _repository.GetEntry("directory-entry-slug");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetAllDirectoryEntries_ShouldReturn_IEnumerableOfDirectoryEntry()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { });

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetAllDirectoryEntries();

        // Assert
        _mockCache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
    }

    [Fact]
    public async Task GetDirectoryEntriesForDirectory_ShouldReturn_CorrectNumberOfDirectories()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetDirectoryEntriesForDirectory("test-directory");

        // Assert
        Assert.Single(response);
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
    }

    [Fact]
    public async Task GetDirectoryEntriesForDirectory_ShouldReturn_Null_IfNoEntriesFound()
    {
        // Arrange
        _mockCache
            .Setup(_ => _.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetDirectoryEntriesForDirectory("non-existant-test-directory");

        // Assert
        Assert.Empty(response);
    }

    [Fact]
    public async Task GetAllDirectoryEntriesFromSource()
    {
        // Arrange
        ContentfulDirectoryEntry contentfulDirectoryEntry = new DirectoryEntryBuilder().WithSlug("a-slug").Build();
        ContentfulCollection<ContentfulDirectoryEntry> collection = new()
        {
            Items = new List<ContentfulDirectoryEntry> { contentfulDirectoryEntry }
        };

        _contentfulClient
            .Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _mockDirectoryEntryContentfulFactory.Setup(_ => _.ToModel(contentfulDirectoryEntry)).Returns(_directoryEntry);

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetAllDirectoryEntriesFromSource();

        // Assert
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
        Assert.Single(response);
    }
}