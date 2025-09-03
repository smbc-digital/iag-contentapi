namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContactUsCategoryContentfulFactoryTests
{
    readonly ContentfulContactUsCategory _contactUsCategory = new ContentfulContactUsCategoryBuilder().Build();
    private readonly ContactUsCategoryContentfulFactory _contactUsCategoryContentfulFactory = new();

    [Fact]
    public void ToModel_ShouldCreateAContactUsIdFromAContentfulContactUsId()
    {
        // Act
        ContactUsCategory contactUsCategory = _contactUsCategoryContentfulFactory.ToModel(_contactUsCategory);

        // Assert
        Assert.IsType<ContactUsCategory>(contactUsCategory);
        Assert.Equal(_contactUsCategory.Title, contactUsCategory.Title);
        Assert.Equal(_contactUsCategory.BodyTextLeft, contactUsCategory.BodyTextLeft);
        Assert.Equal(_contactUsCategory.BodyTextRight, contactUsCategory.BodyTextRight);
        Assert.Equal(_contactUsCategory.Icon, contactUsCategory.Icon);
    }
}