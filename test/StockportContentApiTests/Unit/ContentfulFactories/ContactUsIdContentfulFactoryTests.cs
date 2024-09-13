namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContactUsIdContentfulFactoryTests
{
    private readonly ContentfulContactUsId _contentfulContactUsId;
    private readonly ContactUsIdContentfulFactory _contactUsIdContentfulFactory;

    public ContactUsIdContentfulFactoryTests()
    {
        _contentfulContactUsId = new() { EmailAddress = "test@stockport.gov.uk", Name = "Test email", Slug = "test-email" };
        _contactUsIdContentfulFactory = new();
    }

    [Fact]
    public void ShouldCreateAContactUsIdFromAContentfulContactUsId()
    {
        ContactUsId contactUsId = _contactUsIdContentfulFactory.ToModel(_contentfulContactUsId);
        contactUsId.Should().BeOfType<ContactUsId>();
        contactUsId.Should().BeEquivalentTo(_contentfulContactUsId, o => o.ExcludingMissingMembers());
    }
}
