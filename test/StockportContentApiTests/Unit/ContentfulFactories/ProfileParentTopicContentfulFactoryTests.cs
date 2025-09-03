namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ProfileParentTopicContentfulFactoryTests
{
    private readonly ProfileParentTopicContentfulFactory _profileParentTopicFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory = new();

    public ProfileParentTopicContentfulFactoryTests() =>
        _profileParentTopicFactory = new ProfileParentTopicContentfulFactory(_subItemFactory.Object, _timeProvider.Object);

    [Fact]
    public void ToModel_ShouldReturnProfile()
    {
        // Arrange
        ContentfulProfile contentfulProfile = new ContentfulProfileBuilder().Build();
        contentfulProfile.Breadcrumbs = new List<ContentfulReference> { new ContentfulReferenceBuilder().SystemContentTypeId("topic").Build() };

        // Act
        Topic topic = _profileParentTopicFactory.ToModel(contentfulProfile);

        // Assert
        Assert.IsType<Topic>(topic);
    }

    [Fact]
    public void ToModel_ShouldConvertContentfulProfileToProfile()
    {
        // Arrange
        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .SystemContentTypeId("topic")
            .Build();

        ContentfulProfile contentfulProfile = new ContentfulProfileBuilder().Build();
        contentfulProfile.Breadcrumbs = new List<ContentfulReference> { contentfulReference };
        contentfulProfile.Sys = new SystemProperties { Id = "valid-id" };

        // Act
        Topic topic = _profileParentTopicFactory.ToModel(contentfulProfile);

        // Assert
        Assert.IsType<Topic>(topic);
        Assert.Equal("slug", topic.Slug);
        Assert.Equal("title", topic.Title);
    }

    [Fact]
    public void ToModel_ShouldNotAddLinks()
    {
        // Arrange
        ContentfulProfile contentfulProfile = new ContentfulProfileBuilder().Build();
        contentfulProfile.Image.SystemProperties.LinkType = "Link";
        contentfulProfile.Breadcrumbs.First().Sys.LinkType = "Link";

        // Act
        Topic topic = _profileParentTopicFactory.ToModel(contentfulProfile);

        // Assert
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldReturnNullTopic_If_TopicInBreadcrumbIsNull()
    {
        // Arrange
        ContentfulProfile contentfulProfile = new()
        {
            Breadcrumbs = new List<ContentfulReference>()
        };

        // Act
        Topic topic = _profileParentTopicFactory.ToModel(contentfulProfile);

        // Assert
        Assert.IsType<NullTopic>(topic);
    }

    [Fact]
    public void ToModel_ShouldReturnNullTopic_WhenEntryIsNull()
    {
        // Act
        Topic topic = _profileParentTopicFactory.ToModel(null);

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

        ContentfulProfile contentfulProfile = new ContentfulProfileBuilder().Build();
        contentfulProfile.Breadcrumbs = new List<ContentfulReference> { contentfulReference };

        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem());

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(DateTime.Now);

        // Act
        Topic result = _profileParentTopicFactory.ToModel(contentfulProfile);

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
            .SubItems(new List<ContentfulReference> { new() { Sys = new SystemProperties { Id = "subitem-id" } } })
            .Build();
        
        ContentfulProfile contentfulProfile = new ContentfulProfileBuilder().Build();
        contentfulProfile.Breadcrumbs = new List<ContentfulReference> { contentfulReference };

        SubItem subItem = new()
        {
            Slug = "subitem-slug",
            Title = "subitem title"
        };

        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(subItem);

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(DateTime.Now);

        // Act
        Topic result = _profileParentTopicFactory.ToModel(contentfulProfile);

        // Assert
        Assert.IsType<Topic>(result);
        Assert.NotNull(result.SubItems);
        Assert.Equal("subitem title", result.SubItems.First().Title);
    }
}