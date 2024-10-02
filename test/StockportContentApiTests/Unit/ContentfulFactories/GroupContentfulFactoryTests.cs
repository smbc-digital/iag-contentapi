namespace StockportContentApiTests.Unit.ContentfulFactories;

public class GroupContentfulFactoryTests
{
    private readonly ContentfulGroup _contentfulGroup;
    private readonly GroupContentfulFactory _groupContentfulFactory;
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulOrganisation, Organisation>> _contentfulOrganisationFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>> _contentfulGroupCategoryFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>> _contentfulGroupSubCategoryFactory;
    private readonly Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>> _contentfulGroupBrandingFactory;
    private Mock<ITimeProvider> _timeProvider;
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

    public GroupContentfulFactoryTests()
    {
        _timeProvider = new Mock<ITimeProvider>();

        _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));
        _contentfulOrganisationFactory = new Mock<IContentfulFactory<ContentfulOrganisation, Organisation>>();
        _contentfulGroupCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupCategory, GroupCategory>>();
        _contentfulGroupSubCategoryFactory = new Mock<IContentfulFactory<ContentfulGroupSubCategory, GroupSubCategory>>();
        _contentfulGroupBrandingFactory = new Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>>();
        _contentfulGroup = new ContentfulGroupBuilder().Build();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
        _groupContentfulFactory = new GroupContentfulFactory(_contentfulOrganisationFactory.Object, _contentfulGroupCategoryFactory.Object, _contentfulGroupSubCategoryFactory.Object, _timeProvider.Object, _documentFactory.Object, _contentfulGroupBrandingFactory.Object, _alertFactory.Object);
    }

    [Fact]
    public void ShouldCreateAGroupFromAContentfulGroup()
    {
        // Arrange
        Crumb crumb = new("Stockport Local", string.Empty, "groups");
        GroupCategory category = new("name", "slug", "icon", "imageUrl");
        GroupAdministrators administrators = new()
        {
            Items = new List<GroupAdministratorItems>
            {
                new()
                {
                    Name = "Name",
                    Email = "Email",
                    Permission = "admin"
                }
            }
        };
        MapPosition mapPosition = new()
        {
            Lat = 39,
            Lon = 2
        };
        List<GroupSubCategory> subCategories = new()
        {
            new("name", "slug")
        };
        Organisation organisation = new()
        {
            Title = "Org"
        };
        List<string> suitableFor = new()
        {
            "people"
        };
        Document document = new DocumentBuilder().Build();

        _documentFactory.Setup(_ => _.ToModel(It.IsAny<Asset>())).Returns(document);
        _contentfulGroupCategoryFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulGroupCategory>()))
            .Returns(category);
        _contentfulGroupSubCategoryFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulGroupSubCategory>()))
            .Returns(subCategories.First());
        _contentfulOrganisationFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulOrganisation>()))
            .Returns(organisation);

        // Act
        Group result = _groupContentfulFactory.ToModel(_contentfulGroup);

        // Assert
        result.AbilityLevel.Should().Be("");
        result.AccessibleTransportLink.Should().Be("link");
        result.AdditionalDocuments.Count.Should().Be(1);
        result.AdditionalDocuments.First().Should().BeEquivalentTo(document);
        result.AdditionalInformation.Should().Be("info");
        result.Address.Should().Be("_address");

        result.AgeRange.Count.Should().Be(1);
        result.AgeRange.First().Should().BeEquivalentTo("15-20");

        result.Breadcrumbs.Count.Should().Be(1);
        result.Breadcrumbs.First().Should().BeEquivalentTo(crumb);

        result.CategoriesReference.Count.Should().Be(1);
        result.CategoriesReference.First().Should().BeEquivalentTo(category);

        result.Cost.Count.Should().Be(1);
        result.CostText.Should().Be("");

        result.DateHiddenFrom.Should().Be(DateTime.MinValue);
        result.DateHiddenTo.Should().Be(DateTime.MinValue);
        result.DateLastModified.Should().Be(DateTime.MinValue);

        result.Email.Should().Be("_email");
        result.Description.Should().Be("_description");
        result.Donations.Should().BeFalse();
        result.DonationsText.Should().Be("donText");
        result.DonationsUrl.Should().Be("donUrl");
        result.ImageUrl.Should().Be("image-url.jpg");
        result.GroupAdministrators.Should().BeEquivalentTo(administrators);
        result.MapPosition.Should().BeEquivalentTo(mapPosition);
        result.Name.Should().Be("_name");
        result.Slug.Should().Be("_slug");
        result.PhoneNumber.Should().Be("_phoneNumber");
        result.Website.Should().Be("_website");
        result.Twitter.Should().Be("_twitter");
        result.Facebook.Should().Be("_facebook");
        result.Volunteering.Should().Be(true);
        result.VolunteeringText.Should().Be("text");
        result.SubCategories.Should().BeEquivalentTo(subCategories);
        result.Status.Should().Be("Published");
        result.Organisation.Should().BeEquivalentTo(organisation);
        result.SuitableFor.Should().BeEquivalentTo(suitableFor);
        result.MetaDescription.Should().Be("_metaDescription");
    }
}