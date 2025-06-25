namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SubItemContentfulFactoryTests
{
    private readonly SubItemContentfulFactory _subItemContentfulFactory;
    private readonly Mock<ITimeProvider> _timeProvider = new();

    public SubItemContentfulFactoryTests()
    {
        _timeProvider.Setup(time => time.Now()).Returns(new DateTime(2017, 01, 01));
        _subItemContentfulFactory = new SubItemContentfulFactory(_timeProvider.Object);
    }

    [Fact]
    public void ShouldCreateASubItemFromAContentfulReference()
    {
        // Arrange
        ContentfulReference ContentfulReference = new()
        {
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }
        };

        // Act
        SubItem subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

        // Assert
        Assert.Equal(ContentfulReference.Slug, subItem.Slug);
        Assert.Equal(ContentfulReference.Title, subItem.Title);
        Assert.Equal(ContentfulReference.Icon, subItem.Icon);
        Assert.Equal(ContentfulReference.Teaser, subItem.Teaser);
        Assert.Equal(ContentfulReference.SunriseDate, subItem.SunriseDate);
        Assert.Equal(ContentfulReference.SunsetDate, subItem.SunsetDate);
        Assert.Equal(ContentfulReference.Sys.ContentType.SystemProperties.Id, subItem.Type);
    }

    // TODO: remove start page inconsistency
    [Fact]
    public void ShouldSetStartPageToADifferentIdThanProvided()
    {
        // Arrange
        ContentfulReference ContentfulReference = new()
        {
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
        };

        // Act
        SubItem subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

        // Assert
        Assert.Equal("start-page", subItem.Type);
    }

    [Fact]
    public void ShouldCreateSubItemWithNameForTitleWhenNoTitleProvided()
    {
        // Arrange
        ContentfulReference ContentfulReference = new()
        {
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
        };

        // Act
        SubItem subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

        // Assert
        Assert.Equal(ContentfulReference.Name, subItem.Title);
    }

    [Fact]
    public void ShouldCreateSubItemWithDefaultIconIfNotSet()
    {
        // Arrange
        ContentfulReference ContentfulReference = new()
        {
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } }
        };

        // Act
        SubItem subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

        // Assert
        Assert.Equal("si-default", subItem.Icon);
    }

    [Fact]
    public void ShouldCreateSubItemWithIcon()
    {
        // Arrange
        ContentfulReference ContentfulReference = new()
        {
            Sys = new SystemProperties { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "startPage" } } },
            Icon = "fa-unique"
        };

        // Act
        SubItem subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

        // Assert
        Assert.Equal("fa-unique", subItem.Icon);
    }

    [Fact]
    public void ShouldCreateSubItemWithoutSubItems()
    {
        // Arrange
        ContentfulReference ContentfulReference = new ContentfulReferenceBuilder()
            .Title("custom name")
            .SubItems(null)
            .TertiaryItems(null)
            .SecondaryItems(null)
            .SystemContentTypeId("topic")
            .Build();

        // Act
        SubItem subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

        // Assert
        Assert.NotNull(subItem);
        Assert.IsType<SubItem>(subItem);
        Assert.Equal("custom name", subItem.Title);
        Assert.Empty(subItem.SubItems);
    }

    [Fact]
    public void ShouldCreateSubItemWithSubItems()
    {
        // Arrange
        ContentfulReference ContentfulReference = new ContentfulReferenceBuilder()
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

        SubItem subItem = _subItemContentfulFactory.ToModel(ContentfulReference);

        subItem.SubItems.Should().HaveCount(6);
    }

    [Fact]
    public void ToModel_ShouldHandleGroupsHomepageSlugCorrectly()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .Slug("test-group-homepage")
            .Title("custom name")
            .Title(string.Empty)
            .SubItems(null)
            .TertiaryItems(null)
            .SecondaryItems(null)
            .SystemContentTypeId("groupHomepage")
            .Build();

        // Act
        SubItem subItem = _subItemContentfulFactory.ToModel(contentfulReference);

        // Assert
        subItem.Slug.Should().Be("groups");
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
        SubItem subItem = _subItemContentfulFactory.ToModel(contentfulReference);

        // Assert
        subItem.Icon.Should().Be("si-coin");
    }
}