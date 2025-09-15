namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PrivacyNoticeParentTopicContentfulFactoryTests
{
    private readonly PrivacyNoticeParentTopicContentfulFactory _privacyNoticeParentTopicFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory = new();

    public PrivacyNoticeParentTopicContentfulFactoryTests() =>
        _privacyNoticeParentTopicFactory = new PrivacyNoticeParentTopicContentfulFactory(_subitemFactory.Object, _timeProvider.Object);

    [Fact]
    public void ToModel_ShouldReturnPrivacyNotice()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();
        contentfulPrivacyNotice.Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() };

        // Act
        Topic topic = _privacyNoticeParentTopicFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(topic);
    }

    [Fact]
    public void ToModel_ShouldConvertContentfulPrivacyNoticeToPrivacyNotice()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();
        contentfulPrivacyNotice.Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() };
        contentfulPrivacyNotice.Sys = new SystemProperties { Id = "valid-id" };
       
        // Act
        Topic topic = _privacyNoticeParentTopicFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(topic);
        Assert.Equal("slug", topic.Slug);
        Assert.Equal("title", topic.Title);
    }

    [Fact]
    public void ToModel_ShouldNotAddLinks()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();
        contentfulPrivacyNotice.Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() };
        contentfulPrivacyNotice.Image.SystemProperties.LinkType = "Link";
        contentfulPrivacyNotice.Breadcrumbs.First().Sys.LinkType = "Link";

        // Act
        Topic topic = _privacyNoticeParentTopicFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldReturnNullTopic_If_TopicInBreadcrumbIsNull()
    {
        // Arrange
        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();

        // Act
        Topic topic = _privacyNoticeParentTopicFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<NullTopic>(topic);
    }

    [Fact]
    public void ToModel_ShouldReturnNullTopic_WhenEntryIsNull()
    {
        // Act
        Topic topic = _privacyNoticeParentTopicFactory.ToModel(null);

        // Assert
        Assert.IsType<NullTopic>(topic);
    }

    [Fact]
    public void ToModel_ShouldAddSubItems()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .SystemContentTypeId("topic")
            .SubItems(new List<ContentfulReference> { new() { Sys = new SystemProperties { Id = "subitem-id" } } })
            .Build();

        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();
        contentfulPrivacyNotice.Breadcrumbs = new List<ContentfulReference> { contentfulReference };
        contentfulPrivacyNotice.Sys = new SystemProperties { Id = "valid-id" };

        _subitemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem());

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(DateTime.Now);

        // Act
        Topic result = _privacyNoticeParentTopicFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(result);
        Assert.NotEmpty(result.SubItems);
    }

    [Fact]
    public void ToModel_ShouldHandleLinkToCurrentArticle()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .SystemContentTypeId("topic")
            .SubItems(new List<ContentfulReference> { new() { Title = "title", Sys = new SystemProperties { Id = "valid-id" } } })
            .Build();

        ContentfulPrivacyNotice contentfulPrivacyNotice = new ContentfulPrivacyNoticeBuilder().Build();
        contentfulPrivacyNotice.Breadcrumbs = new List<ContentfulReference> { contentfulReference };
        contentfulPrivacyNotice.Sys = new SystemProperties { Id = "valid-id" };

        SubItem subItem = new()
        {
            Slug = "subitem-slug",
            Title = "subitem title"
        };

        _subitemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(subItem);

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(DateTime.Now);

        // Act
        Topic result = _privacyNoticeParentTopicFactory.ToModel(contentfulPrivacyNotice);

        // Assert
        Assert.IsType<Topic>(result);
        Assert.NotNull(result.SubItems);
        Assert.Equal("subitem title", result.SubItems.First().Title);
    }
}