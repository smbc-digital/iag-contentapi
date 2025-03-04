namespace StockportContentApiTests.Unit.Repositories;

public class OrganisationRepositoryTests
{
    private readonly OrganisationRepository _repository;
    private readonly Mock<IContentfulClient> _contentfulClient;

    public OrganisationRepositoryTests()
    {
        ContentfulConfig config = new ContentfulConfig("test")
            .Add("DELIVERY_URL", "https://fake.url")
            .Add("TEST_SPACE", "SPACE")
            .Add("TEST_ACCESS_KEY", "KEY")
            .Add("TEST_MANAGEMENT_KEY", "KEY")
            .Add("TEST_ENVIRONMENT", "master")
            .Build();

        OrganisationContentfulFactory contentfulFactory = new();

        Mock<IContentfulClientManager> contentfulClientManager = new();
        _contentfulClient = new Mock<IContentfulClient>();
        contentfulClientManager.Setup(o => o.GetClient(config)).Returns(_contentfulClient.Object);

        Mock<Func<string, string, IGroupRepository>> _groupRepository = new();
        List<Group> groups = new();
        Organisation organisation = new() { Slug = "slug", Title = "Title" };
        groups.Add(new GroupBuilder().Organisation(organisation).Build());
        groups.Add(new GroupBuilder().Organisation(organisation).Build());
        groups.Add(new GroupBuilder().Organisation(organisation).Build());

        _repository = new OrganisationRepository
        (
            config,
            contentfulFactory,
            contentfulClientManager.Object,
            _groupRepository.Object
        );
    }

    [Fact(Skip = "Hot fix, it will be deprecated soon")]
    public void ShouldGetOrganisation()
    {
        // Arrange          
        ContentfulOrganisation contentfulOrganisation = new()
        {
            AboutUs = "about us",
            Email = "Email",
            Phone = "Phone",
            Slug = "slug",
            Title = "title",
            Volunteering = true,
            VolunteeringText = "test"
        };

        QueryBuilder<ContentfulOrganisation> builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", "slug");
        ContentfulCollection<ContentfulOrganisation> contentfulCollection = new()
        {
            Items = new List<ContentfulOrganisation> { contentfulOrganisation }
        };

        _contentfulClient.Setup(o => o.GetEntries(It.Is<QueryBuilder<ContentfulOrganisation>>(q => q.Build().Equals(builder.Build())), It.IsAny<CancellationToken>()))
            .ReturnsAsync(contentfulCollection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetOrganisation("slug", new ContentfulConfig("stockportgov"), new CacheKeyConfig("stockportgov")));
        Organisation organisation = response.Get<Organisation>();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        organisation.Slug.Should().Be(contentfulOrganisation.Slug);
    }

    [Fact]
    public void ShouldReturn404ForNonExistentSlug()
    {
        // Arrange
        const string slug = "invalid-url";

        ContentfulCollection<ContentfulOrganisation> collection = new()
        {
            Items = new List<ContentfulOrganisation>()
        };

        QueryBuilder<ContentfulOrganisation> builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);
        _contentfulClient.Setup(o => o.GetEntries(
            It.IsAny<QueryBuilder<ContentfulOrganisation>>(),
                    It.IsAny<CancellationToken>()))
            .ReturnsAsync(collection);

        // Act
        HttpResponse response = AsyncTestHelper.Resolve(_repository.GetOrganisation(slug, new ContentfulConfig("stockportgov"), new CacheKeyConfig("stockportgov")));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}