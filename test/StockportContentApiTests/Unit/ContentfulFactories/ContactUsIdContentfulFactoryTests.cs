namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContactUsIdContentfulFactoryTests
{
    readonly ContentfulContactUsId _contentfulContactUsId = new ContentfulContactUsIdBuilder().Build();
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
    }
}