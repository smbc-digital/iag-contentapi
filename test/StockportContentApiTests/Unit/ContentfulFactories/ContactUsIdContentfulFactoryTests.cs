namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContactUsIdContentfulFactoryTests
{
    private readonly ContentfulContactUsId _contentfulContactUsId = new() { EmailAddress = "test@stockport.gov.uk", Name = "Test email", Slug = "test-email" };
    private readonly ContactUsIdContentfulFactory _contactUsIdContentfulFactory = new();


    [Fact]
    public void ToModel_ShouldCreateAContactUsIdFromAContentfulContactUsId()
    {
        // Act
        ContactUsId contactUsId = _contactUsIdContentfulFactory.ToModel(_contentfulContactUsId);

        // Assert
        Assert.IsType<ContactUsId>(contactUsId);
        Assert.Equal(_contentfulContactUsId.Name, contactUsId.Name);
        Assert.Equal(_contentfulContactUsId.Slug, contactUsId.Slug);
        Assert.Equal(_contentfulContactUsId.EmailAddress, contactUsId.EmailAddress);
        Assert.Equal(_contentfulContactUsId.SuccessPageButtonText, contactUsId.SuccessPageButtonText);
        Assert.Equal(_contentfulContactUsId.SuccessPageReturnUrl, contactUsId.SuccessPageReturnUrl);
    }
}