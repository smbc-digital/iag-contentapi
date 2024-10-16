namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContactUsCategoryontentfulFactoryTests
{
    private readonly ContentfulContactUsCategory _contentfulContactUsCategory = new() { Title = "title", BodyTextLeft = "body text left", BodyTextRight = "body text right", Icon = "icon" };
    private readonly ContactUsCategoryContentfulFactory _contactUsCategoryContentfulFactory = new();

    [Fact]
    public void ToModel_ShouldCreateAContactUsIdFromAContentfulContactUsId()
    {
        // Act
        ContactUsCategory contactUsCategory = _contactUsCategoryContentfulFactory.ToModel(_contentfulContactUsCategory);

        // Assert
        Assert.IsType<ContactUsCategory>(contactUsCategory);
        Assert.Equal(_contentfulContactUsCategory.Title, contactUsCategory.Title);
        Assert.Equal(_contentfulContactUsCategory.BodyTextLeft, contactUsCategory.BodyTextLeft);
        Assert.Equal(_contentfulContactUsCategory.BodyTextRight, contactUsCategory.BodyTextRight);
        Assert.Equal(_contentfulContactUsCategory.Icon, contactUsCategory.Icon);
    }
}