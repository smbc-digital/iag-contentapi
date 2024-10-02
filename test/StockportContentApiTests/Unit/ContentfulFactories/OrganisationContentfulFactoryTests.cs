namespace StockportContentApiTests.Unit.ContentfulFactories;

public class OrganisationContentfulFactoryTests
{
    private readonly ContentfulOrganisation _contentfulOrganisation;
    private readonly OrganisationContentfulFactory _organisationContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulOrganisation, Organisation>> _contentfulOrganisationFactory;

    public OrganisationContentfulFactoryTests()
    {
        _contentfulOrganisationFactory = new Mock<IContentfulFactory<ContentfulOrganisation, Organisation>>();
        _contentfulOrganisation = new ContentfulOrganisation
        {
            AboutUs = "about us",
            Email = "email",
            Image = null,
            Phone = "phone",
            Slug = "slug",
            Title = "title",
            Volunteering = true,
            VolunteeringText = "help wanted"
        };

        _organisationContentfulFactory = new OrganisationContentfulFactory();
    }

    [Fact]
    public void ShouldCreateAGroupFromAContentfulGroup()
    {
        Organisation group = _organisationContentfulFactory.ToModel(_contentfulOrganisation);
        group.Should().BeEquivalentTo(_contentfulOrganisation, o => o.ExcludingMissingMembers());
    }
}
