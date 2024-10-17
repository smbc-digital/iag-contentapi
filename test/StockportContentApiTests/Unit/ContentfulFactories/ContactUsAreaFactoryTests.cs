namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ContactUsAreaFactoryTests
{
    private readonly ContactUsAreaContentfulFactory _factory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _mockSubitemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new();
    private readonly Mock<ITimeProvider> _mockTimeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _mockAlertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulContactUsCategory, ContactUsCategory>> _mockContactUsCategoryFactory = new();

    public ContactUsAreaFactoryTests()
    {
        _factory = new ContactUsAreaContentfulFactory(_mockSubitemFactory.Object,
            _mockCrumbFactory.Object,
            _mockTimeProvider.Object,
            _mockAlertFactory.Object,
            _mockContactUsCategoryFactory.Object);
    }

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
        Assert.NotNull(result.CategoriesTitle);
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

        _mockSubitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem());

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
        Assert.NotNull(result.CategoriesTitle);
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

        _mockSubitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem());

        // Act
        ContactUsArea result = _factory.ToModel(entry);
        
        // Assert
        Assert.Single(result.Breadcrumbs);
        Assert.Single(result.Alerts);
        Assert.Single(result.PrimaryItems);
        Assert.Single(result.ContactUsCategories);
        Assert.NotNull(result.Slug);
        Assert.NotNull(result.Title);
        Assert.NotNull(result.CategoriesTitle);
        Assert.NotNull(result.InsetTextTitle);
        Assert.NotNull(result.InsetTextBody);
    }
}