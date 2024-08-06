namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PrivacyNoticeParentTopicContentfulFactoryTest
{
    private readonly PrivacyNoticeParentTopicContentfulFactory _privacyNoticeParentTopicContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _mockCrumbFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory = new();

    public PrivacyNoticeParentTopicContentfulFactoryTest()
    {
        _privacyNoticeParentTopicContentfulFactory = new PrivacyNoticeParentTopicContentfulFactory(_subitemFactory.Object, _timeProvider.Object);
    }

    [Fact]
    public void ToModel_ShouldReturnPrivacyNotice()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = "test-slug",
            Title = "test-title",
            Category = "test-category",
            Purpose = "test-purpose",
            TypeOfData = "test-type-of-data",
            Legislation = "test-legislation",
            Obtained = "test-obtained",
            ExternallyShared = "test-externally-shared",
            RetentionPeriod = "test-retention-period",
            OutsideEu = false,
            AutomatedDecision = false,
            Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() }
        };

        // Act
        Topic topic = _privacyNoticeParentTopicContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(topic);
    }

    [Fact]
    public void ToModel_ShouldConvertContentfulPrivacyNoticeToPrivacyNotice()
    {
        // Arrange
        ContentfulReference contentfulReference = new()
        {
            Slug = "slug",
            Name = "name",
            Sys = new SystemProperties { Id = "valid-id", ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "topic" } } },
            SubItems = new List<ContentfulReference>(),
            SecondaryItems = new List<ContentfulReference>()
        };

        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Breadcrumbs = new List<ContentfulReference> { contentfulReference },
            Sys = new SystemProperties { Id = "valid-id" }
        };

        // Act
        Topic topic = _privacyNoticeParentTopicContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(topic);
        Assert.Equal("slug", topic.Slug);
        Assert.Equal("name", topic.Name);
    }

    [Fact]
    public void ToModel_ShouldNotAddLinks()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Slug = "test-slug",
            Title = "test-title",
            Category = "test-category",
            Purpose = "test-purpose",
            TypeOfData = "test-type-of-data",
            Legislation = "test-legislation",
            Obtained = "test-obtained",
            ExternallyShared = "test-externally-shared",
            RetentionPeriod = "test-retention-period",
            OutsideEu = false,
            AutomatedDecision = false,
            Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() }
        };

        contentfulPrivacyNotice.Image.SystemProperties.LinkType = "Link";
        contentfulPrivacyNotice.Breadcrumbs.First().Sys.LinkType = "Link";

        // Act
        Topic topic = _privacyNoticeParentTopicContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        _mockCrumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldReturnNullTopic_If_TopicInBreadcrumbIsNull()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Breadcrumbs = new List<ContentfulReference>()
        };

        // Act
        Topic topic = _privacyNoticeParentTopicContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<NullTopic>(topic);
    }

    [Fact]
    public void ToModel_ShouldReturnNullTopic_WhenEntryIsNull()
    {
        // Act
        Topic topic = _privacyNoticeParentTopicContentfulFactory.ToModel(null);

        // Assert
        Assert.IsType<NullTopic>(topic);
    }

    [Fact]
    public void ToModel_ShouldAddSubItems()
    {
        // Arrange
        ContentfulReference contentfulReference = new()
        {
            Sys = new SystemProperties { Id = "valid-id", ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "topic" } } },
            SubItems = new List<ContentfulReference> { new() { Sys = new SystemProperties { Id = "subitem-id" } } },
            SecondaryItems = new List<ContentfulReference>()
        };

        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Breadcrumbs = new List<ContentfulReference> { contentfulReference },
            Sys = new SystemProperties { Id = "valid-id" }
        };

        _subitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem());
        _timeProvider.Setup(_ => _.Now()).Returns(DateTime.Now);

        // Act
        Topic result = _privacyNoticeParentTopicContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(result);
        Assert.NotEmpty(result.SubItems);
    }

    [Fact]
    public void ToModel_ShouldHandleLinkToCurrentArticle()
    {
        // Arrange
        ContentfulReference contentfulReference = new()
        {
            Sys = new SystemProperties { Id = "valid-id", ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "topic" } } },
            SubItems = new List<ContentfulReference> { new() { Title = "title", Sys = new SystemProperties { Id = "valid-id" } } },
            SecondaryItems = new List<ContentfulReference>()
        };

        ContentfulPrivacyNotice contentfulPrivacyNotice = new()
        {
            Breadcrumbs = new List<ContentfulReference> { contentfulReference },
            Sys = new SystemProperties { Id = "valid-id" },
            Icon = "icon",
            Title = "title",
            SunriseDate = DateTime.Now.AddDays(-1),
            SunsetDate = DateTime.Now.AddDays(1),
            Slug = "slug",
            Teaser = "teaser"
        };

        SubItem subItem = new(){
            Slug = "subitem-slug",
            Title = "subitem title"
        };

        _subitemFactory.Setup(_ => _.ToModel(It.IsAny<ContentfulReference>())).Returns(subItem);
        _timeProvider.Setup(_ => _.Now()).Returns(DateTime.Now);

        // Act
        Topic result = _privacyNoticeParentTopicContentfulFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(result);
        Assert.NotNull(result.SubItems);
        Assert.Equal("subitem title", result.SubItems.First().Title);
    }
}