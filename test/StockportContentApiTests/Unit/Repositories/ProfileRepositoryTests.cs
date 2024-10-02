namespace StockportContentApiTests.Unit.Repositories;

public class ProfileRepositoryTests
{
    private readonly ProfileRepository _repository;
    private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();
    private readonly Mock<IContentfulClient> _client;
    private readonly Mock<IContentfulClientManager> _contentfulClientManager = new();

    public ProfileRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        _client = new Mock<IContentfulClient>();
        _contentfulClientManager.Setup(_ => _.GetClient(config)).Returns(_client.Object);
        _repository = new ProfileRepository(config, _contentfulClientManager.Object, _profileFactory.Object);
    }

    [Fact]
    public void GetsProfileForProfileSlug()
    {
        // Arrange
        ContentfulProfile contentfulProfile = new ContentfulProfileBuilder().Slug("a-slug").Build();
        ContentfulCollection<ContentfulProfile> collection = new()
        {
            Items = new List<ContentfulProfile> { contentfulProfile }
        };

        Profile profile = new()
        {
            Title = "title",
            Slug = "slug",
            Subtitle = "subtitle",
            Teaser = "teaser",
            Image = new MediaAsset(),
            Body = "body",
            ImageCaption = "imageCaption",
            Breadcrumbs = new List<Crumb>
            {
                new("title", "slug", "type")
            },
            Alerts = new List<Alert>
            {
                new("title",
                    "subheading",
                    "body",
                    "severity",
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    "slug",
                    false, string.Empty)
            },
            TriviaSubheading = "trivia heading",
            TriviaSection = new List<Trivia>(),
            InlineQuotes = new List<InlineQuote>(),
            Author = "author",
            Subject = "subject",
            Colour = EColourScheme.Pink
        };

        QueryBuilder<ContentfulProfile> builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", "a-slug").Include(2);

        _client.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulProfile>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);
        _profileFactory.Setup(_ => _.ToModel(contentfulProfile)).Returns(profile);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetProfile("a-slug"));
        Profile responseProfile = response.Get<Profile>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(profile, responseProfile);
    }

    [Fact]
    public void Return404WhenProfileWhenItemsDontExist()
    {
        // Arrange
        ContentfulCollection<ContentfulProfile> collection = new()
        {
            Items = new List<ContentfulProfile>()
        };

        QueryBuilder<ContentfulProfile> builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", "not-found").Include(1);
        _client.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetProfile("not-found"));

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        Assert.Equal("No profile found for 'not-found'", response.Error);
    }

    [Fact]
    public async Task GetProfile_ShouldGetEntries()
    {
        // Arrange
        _client.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentfulCollection<ContentfulProfile>
                {
                    Items = new List<ContentfulProfile>()
                });

        // Act
        await _repository.GetProfile("fake slug");

        // Assert
        _client.Verify(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()), Times.Once());
    }

    [Fact]
    public async Task GetProfile_ShouldReturnFailureWhenNoEntries()
    {
        // Arrange
        _client.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentfulCollection<ContentfulProfile>
                {
                    Items = new List<ContentfulProfile>()
                });

        // Act
        HttpResponse response = await _repository.GetProfile("fake slug");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task GetProfile_ShouldReturnSuccessWhenEntriesExist()
    {
        // Arrange
        _client.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ContentfulCollection<ContentfulProfile>
                {
                    Items = new List<ContentfulProfile>(){
                        new(){
                            Slug = "slug"
                        }
                    }
                });

        // Act
        HttpResponse response = await _repository.GetProfile("fake slug");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async void Get_ShouldReturnSuccessfulResponse()
    {
        // Arrange
        ContentfulProfile contentfulProfileA = new ContentfulProfileBuilder().Slug("a-slug").Build();
        ContentfulProfile contentfulProfileB = new ContentfulProfileBuilder().Slug("b-slug").Build();
        ContentfulProfile contentfulProfileC = new ContentfulProfileBuilder().Slug("c-slug").Build();
        ContentfulCollection<ContentfulProfile> collection = new()
        {
            Items = new List<ContentfulProfile> { contentfulProfileA, contentfulProfileB, contentfulProfileC }
        };

        QueryBuilder<ContentfulProfile> builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").Include(1);

        _client.Setup(_ => _.GetEntries(It.Is<QueryBuilder<ContentfulProfile>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
                .ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.Get();
        IEnumerable<Profile> responseProfile = response.Get<IEnumerable<Profile>>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(collection.Items.Count(), responseProfile.ToList().Count);
    }

    [Fact]
    public async Task Get_ShouldReturnNotFoundResponse_IfProfileADoesNotExist()
    {
        // Arrange
        ContentfulCollection<ContentfulProfile> collection = new()
        {
            Items = new List<ContentfulProfile>()
        };

        _client.Setup(_ => _.GetEntries(It.IsAny<QueryBuilder<ContentfulProfile>>(), It.IsAny<CancellationToken>())).ReturnsAsync(collection);

        // Act
        HttpResponse response = await _repository.Get();

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}