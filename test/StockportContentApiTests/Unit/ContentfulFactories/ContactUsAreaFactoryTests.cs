namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContactUsAreaFactoryTests
{
    private readonly ContactUsAreaContentfulFactory _factory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>> _contactUsCategoryFactory = new();

    public ContactUsAreaFactoryTests() =>
        _factory = new ContactUsAreaContentfulFactory(_subItemFactory.Object,
                                                    _crumbFactory.Object,
                                                    _timeProvider.Object,
                                                    _alertFactory.Object,
                                                    _contactUsCategoryFactory.Object);

    [Fact]
    public void ShouldCreate_ValidContentfulContactUsAreModel()
    {
        // Arrange
        ContentfulContactUsArea entry = new ContentfulContactUsAreaBuilder().Build();

        // Act
        ContactUsArea result = _factory.ToModel(entry);

        // Assert
        Assert.NotNull(result.Breadcrumbs);
        Assert.NotNull(result.Alerts);
        Assert.NotNull(result.InsetTextTitle);
        Assert.NotNull(result.InsetTextBody);
        Assert.NotNull(result.PrimaryItems);
        Assert.NotNull(result.Slug);
        Assert.NotNull(result.Title);
        Assert.Equal("title", result.Title);
        Assert.Equal("slug", result.Slug);
        Assert.NotNull(result.ContactUsCategories);
        Assert.Equal("metaDescription", result.MetaDescription);
        Assert.Equal("insetTextTitle", result.InsetTextTitle);
        Assert.Equal("insetTextBody", result.InsetTextBody);
    }

    [Fact]
    public void ShouldCreate_ValidContentfulContactUsAreModel_WithPrimaryItems()
    {
        // Arrange
        List<ContentfulReference> primaryItems = new() { new() { } };
        ContentfulContactUsArea entry = new ContentfulContactUsAreaBuilder()
                            .PrimaryItems(primaryItems)
                            .Build();

        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem());

        // Act
        ContactUsArea result = _factory.ToModel(entry);
        
        // Assert
        Assert.NotNull(result.Breadcrumbs);
        Assert.NotNull(result.Alerts);
        Assert.NotNull(result.InsetTextTitle);
        Assert.NotNull(result.InsetTextBody);
        Assert.Single(result.PrimaryItems);
        Assert.NotNull(result.Slug);
        Assert.NotNull(result.Title);
    }

    [Fact]
    public void ShouldCreate_ValidContentfulContactUsAreaModel_WithAllItems()
    {
        // Arrange
        List<ContentfulReference> primaryItems = new() { new() };
        List<ContentfulReference> breadcrumbs = new() { new() };
        List<ContentfulAlert> alerts = new() { new() };

        List<ContentfulContactUsCategory> contactUsCategories = new()
        {
            new ContentfulContactUsCategory()
        };

        ContentfulContactUsArea entry = new ContentfulContactUsAreaBuilder()
                            .PrimaryItems(primaryItems)
                            .Breadcrumbs(breadcrumbs)
                            .Alerts(alerts)
                            .ContentfulContactUsCategories(contactUsCategories)
                            .Build();

        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem());

        // Act
        ContactUsArea result = _factory.ToModel(entry);
        
        // Assert
        Assert.Single(result.Breadcrumbs);
        Assert.Single(result.Alerts);
        Assert.Single(result.PrimaryItems);
        Assert.Single(result.ContactUsCategories);
        Assert.NotNull(result.Slug);
        Assert.NotNull(result.Title);
        Assert.NotNull(result.InsetTextTitle);
        Assert.NotNull(result.InsetTextBody);
    }
}