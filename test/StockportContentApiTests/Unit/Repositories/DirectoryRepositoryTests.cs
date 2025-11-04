using Microsoft.Extensions.Options;
using Directory = StockportContentApi.Models.Directory;

namespace StockportContentApiTests.Unit.Repositories;

public class DirectoryRepositoryTests
{
    private readonly DirectoryRepository _repository;
    private readonly Directory _directory;
    private readonly DirectoryEntry _directoryEntry;
    private readonly Mock<IContentfulClient> _contentfulClient = new();
    private readonly Mock<IContentfulFactory<ContentfulDirectory, Directory>> _directoryFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>> _directoryEntryFactory = new();
    private readonly Mock<ICache> _cache = new();
    private readonly Mock<IOptions<RedisExpiryConfiguration>> _options = new();

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
            .Setup(contentfulClientManager => contentfulClientManager.GetClient(config))
            .Returns(_contentfulClient.Object);

        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IList<ContentfulDirectory>>>>(), It.IsAny<int>()))
            .ReturnsAsync((IList<ContentfulDirectory>)null);

        _options
            .Setup(options => options.Value)
            .Returns(new RedisExpiryConfiguration { Directory = 1 });

        _repository = new DirectoryRepository(config,
                                            contentfulClientManager.Object,
                                            _directoryFactory.Object,
                                            _directoryEntryFactory.Object,
                                            _cache.Object,
                                            _options.Object);
}

    [Fact]
    public async Task GetDirectoryFromSource_WithSlug_Should_ReturnCorrectType()
    {
        // Arrange
        ContentfulDirectory contentfulDirectory = new DirectoryBuilder().WithSlug("a-slug").Build();
        ContentfulCollection<ContentfulDirectory> collection = new()
        {
            Items = new List<ContentfulDirectory> { contentfulDirectory }
        };

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>() });

        _directoryFactory
            .Setup(directoryFactory => directoryFactory.ToModel(contentfulDirectory))
            .Returns(_directory);

        // Act
        Directory response = await _repository.GetDirectoryFromSource("a-slug", 2, "tagId");

        // Assert
        Assert.IsType<Directory>(response);
        Assert.Equal(response, _directory);
    }

    [Fact]
    public async Task GetDirectoryFromSource_WithSlug_Should_ReturnNull_WhenDepthLimitExceeded()
    {
        // Act
        Directory response = await _repository.GetDirectoryFromSource("slug", 6, "tagId");

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
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(),It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>() });

        _directoryFactory
            .Setup(directoryFactory => directoryFactory.ToModel(contentfulDirectory))
            .Returns(_directory);

        // Act
        Directory response = await _repository.GetDirectoryFromSource("non-existant-slug", 2, "tagId");

        // Assert
        Assert.Null(response);
    }

    [Fact]
    public async Task Get_WithSlug_Should_GetDirectoryEntries()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()))
            .ReturnsAsync(_directory);

        // Act
        HttpResponse response = await _repository.Get("directory-slug", "tagId");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<Directory>(), _directory);
        _cache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task Get_WithSlug_Should_Return_FailedNotFound_IfDirectory_DoesNot_Exist()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()))
            .ReturnsAsync((Directory)null);

        // Act
        HttpResponse response = await _repository.Get("directory-slug", "tagId");

        // Assert
        Assert.IsType<HttpResponse>(response);
        _cache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()), Times.Once);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task Get_Should_Return_FailedNotFound_IfNoEntriesExist()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<Directory>>>(), It.IsAny<int>()))
            .ReturnsAsync((Directory)null);

        // Act
        HttpResponse response = await _repository.Get("tagId");

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
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectory>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _contentfulClient
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ContentfulCollection<ContentfulDirectoryEntry> { Items = new List<ContentfulDirectoryEntry>() });

        _directoryFactory
            .Setup(directoryFactory => directoryFactory.ToModel(contentfulDirectory))
            .Returns(_directory);

        // Act
        HttpResponse response = await _repository.Get("tagId");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(response.Get<List<Directory>>(), new List<Directory>() { _directory });
    }

    [Fact]
    public async Task GetEntry_WithSlug_Should_ReturnSuccess()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        // Act
        HttpResponse response = await _repository.GetEntry("directory-entry-slug", "tagId");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(response.Get<DirectoryEntry>(), _directoryEntry);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        _cache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetEntry_WithSlug_ShouldReturn_NotFound_If_DirectoryEntryDoesNotExist()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { });

        // Act
        HttpResponse response = await _repository.GetEntry("directory-entry-slug", "tagId");

        // Assert
        Assert.IsType<HttpResponse>(response);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        _cache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GetAllDirectoryEntries_ShouldReturn_IEnumerableOfDirectoryEntry()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { });

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetAllDirectoryEntries("tagId");

        // Assert
        _cache.Verify(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()), Times.Once);
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
    }

    [Fact]
    public async Task GetDirectoryEntriesForDirectory_ShouldReturn_CorrectNumberOfDirectories()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetDirectoryEntriesForDirectory("test-directory", "tagId");

        // Assert
        Assert.Single(response);
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
    }

    [Fact]
    public async Task GetDirectoryEntriesForDirectory_ShouldReturn_Null_IfNoEntriesFound()
    {
        // Arrange
        _cache
            .Setup(cache => cache.GetFromCacheOrDirectlyAsync(It.IsAny<string>(), It.IsAny<Func<Task<IEnumerable<DirectoryEntry>>>>(), It.IsAny<int>()))
            .ReturnsAsync(new List<DirectoryEntry> { _directoryEntry });

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetDirectoryEntriesForDirectory("non-existant-test-directory", "tagId");

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
            .Setup(contentfulClient => contentfulClient.GetEntries(It.IsAny<QueryBuilder<ContentfulDirectoryEntry>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        _directoryEntryFactory
            .Setup(directoryFactory => directoryFactory.ToModel(contentfulDirectoryEntry))
            .Returns(_directoryEntry);

        // Act
        IEnumerable<DirectoryEntry> response = await _repository.GetAllDirectoryEntriesFromSource("tagId");

        // Assert
        Assert.IsAssignableFrom<IEnumerable<DirectoryEntry>>(response);
        Assert.Single(response);
    }
}