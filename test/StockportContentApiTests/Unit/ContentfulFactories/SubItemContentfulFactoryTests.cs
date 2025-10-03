namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SubItemContentfulFactoryTests
{
    private readonly SubItemContentfulFactory _subItemFactory;
    private readonly Mock<ITimeProvider> _timeProvider = new();

    public SubItemContentfulFactoryTests()
    {
        _timeProvider
            .Setup(time => time.Now())
            .Returns(new DateTime(2017, 01, 01));

        _subItemFactory = new SubItemContentfulFactory(_timeProvider.Object);
    }

    [Fact]
    public void ShouldCreateASubItemFromAContentfulReference()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder().SystemContentTypeId("id").Build();

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Slug, subItem.Slug);
        Assert.Equal(contentfulReference.Title, subItem.Title);
        Assert.Equal(contentfulReference.Icon, subItem.Icon);
        Assert.Equal(contentfulReference.Teaser, subItem.Teaser);
        Assert.Equal(contentfulReference.SunriseDate, subItem.SunriseDate);
        Assert.Equal(contentfulReference.SunsetDate, subItem.SunsetDate);
        Assert.Equal(contentfulReference.Sys.ContentType.SystemProperties.Id, subItem.Type);
    }

    // TODO: remove start page inconsistency
    [Fact]
    public void ShouldSetStartPageToADifferentIdThanProvided()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder().SystemContentTypeId("startPage").Build();

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.Equal("start-page", subItem.Type);
    }

    [Fact]
    public void ShouldCreateSubItemWithNameForTitleWhenNoTitleProvided()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder().SystemContentTypeId("startPage").Build();
        contentfulReference.Name = "title";

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Name, subItem.Title);
    }

    [Fact]
    public void ShouldCreateSubItemWithDefaultIconIfNotSet()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder().SystemContentTypeId("startPage").Build();

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Icon, subItem.Icon);
    }

    [Fact]
    public void ShouldCreateSubItemWithIcon()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder().Icon("fa-unique").SystemContentTypeId("startPage").Build();

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Icon, subItem.Icon);
    }

    [Fact]
    public void ShouldCreateSubItemWithoutSubItems()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .Title("custom name")
            .SubItems(null)
            .TertiaryItems(null)
            .SecondaryItems(null)
            .SystemContentTypeId("topic")
            .Build();

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.NotNull(subItem);
        Assert.IsType<SubItem>(subItem);
        Assert.Equal(contentfulReference.Title, subItem.Title);
        Assert.Empty(subItem.SubItems);
    }

    [Fact]
    public void ShouldCreateSubItemWithSubItems()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .SubItems(new List<ContentfulReference>()
            {
                new()
                {
                    Sys = new SystemProperties() { Type = "Entry", ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}} }
                },
                new()
                {
                    Sys = new SystemProperties() { Type = "Entry", ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}}  }
                }
            })
            .TertiaryItems(new List<ContentfulReference>()
            {
                new()
                {
                    Sys = new SystemProperties() { Type = "Entry", ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}} }
                },
                new()
                {
                    Sys = new SystemProperties() { Type = "Entry", ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}} }
                }
            })
            .SecondaryItems(new List<ContentfulReference>()
            {
                new()
                {
                    Sys = new SystemProperties() { Type = "Entry", ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}} }
                },
                new()
                {
                    Sys = new SystemProperties() { Type = "Entry", ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "topic"}} }
                }
            })
            .Build();

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.Equal(6, subItem.SubItems.Count);
    }

    [Fact]
    public void ToModel_ShouldSetPaymentsGroupIconCorrectly_WhenNonSet()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .Slug("test-payment-group")
            .Title("custom name")
            .Title("title")
            .SubItems(null)
            .TertiaryItems(null)
            .SecondaryItems(null)
            .Icon(null)
            .SystemContentTypeId("payment")
            .Build();

        // Act
        SubItem subItem = _subItemFactory.ToModel(contentfulReference);

        // Assert
        Assert.Equal(contentfulReference.Icon, subItem.Icon);
    }
}