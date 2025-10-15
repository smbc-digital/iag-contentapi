namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ParentTopicContentfulFactoryTests
{
    private readonly ParentTopicContentfulFactory _parentTopicFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subItemFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();

    public ParentTopicContentfulFactoryTests()
    {
        _subItemFactory
            .Setup(subItem => subItem.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug",
                                "title",
                                "teaser",
                                "teaser image",
                                "icon",
                                "type",
                                DateTime.MinValue,
                                DateTime.MaxValue,
                                "image",
                                new(),
                                EColourScheme.Green,
                                new List<string>()));

        _timeProvider
            .Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2017, 01, 02));

        _parentTopicFactory = new(_subItemFactory.Object, _timeProvider.Object);
    }

    [Fact]
    public void ToModel_ShouldReturnATopicFromAContentfulArticleBasedOnTheBreadcrumbs()
    {
        // Arrange
        List<ContentfulReference> subItemEntry = new()
        {
            new ContentfulReferenceBuilder().Slug("sub-slug").Build()
        };

        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .Title("test topic")
            .Slug("test-topic")
            .SubItems(subItemEntry)
            .SystemContentTypeId("topic")
            .Build();

        ContentfulArticle contentfulArticleEntry = new ContentfulArticleBuilder().Breadcrumbs(new() { contentfulReference }).Build();

        // Act
        Topic result = _parentTopicFactory.ToModel(contentfulArticleEntry);

        // Assert
        Assert.Equal("test topic", result.Title);
        Assert.Equal("test-topic", result.Slug);
        Assert.Single(result.SubItems);
    }

    [Fact]
    public void ToModel_ShouldReturnNullTopicIfBreadcrumbDoesNotHaveTypeOfTopic()
    {
        // Arrange
        List<ContentfulReference> subItemEntry = new()
        {
            new ContentfulReferenceBuilder().Slug("sub-slug").Build()
        };

        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .Title("test topic")
            .Slug("test-topic")
            .SubItems(subItemEntry)
            .SystemContentTypeId("id")
            .Build();

        ContentfulArticle contentfulArticleEntry = new ContentfulArticleBuilder().Breadcrumbs(new() { contentfulReference }).Build();

        // Act
        Topic result = _parentTopicFactory.ToModel(contentfulArticleEntry);

        // Assert
        Assert.IsType<NullTopic>(result);
    }

    [Fact]
    public void ShouldReturnNullTopicIfNoBreadcrumbs()
    {
        // Arrange
        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder()
            .Breadcrumbs(new())
            .Build();

        // Act
        Topic result = _parentTopicFactory.ToModel(contentfulArticle);

        // Assert
        Assert.IsType<NullTopic>(result);
    }

    [Fact]
    public void ShouldReturnTopicWithFirstSubItemOfTheArticle()
    {
        // Arrange
        ContentfulReference subItemEntry = new ContentfulReferenceBuilder()
            .Slug("sub-slug")
            .SystemId("same-id-as-article")
            .Build();

        ContentfulReference subItemEntryOther = new ContentfulReferenceBuilder()
            .Slug("sub-slug")
            .SystemId("not-same-id-as-article")
            .Build();

        List<ContentfulReference> subItemEntryList = new()
        {
            subItemEntry,
            subItemEntryOther
        };

        ContentfulReference contentfulReference = new ContentfulReferenceBuilder()
            .Title("test topic")
            .Slug("test-topic")
            .SubItems(subItemEntryList)
            .SystemContentTypeId("topic")
            .Build();

        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder()
            .Breadcrumbs(new()
            {
                contentfulReference
            })
            .Title("title")
            .Slug("slug")
            .SystemId("same-id-as-article")
            .Build();

        _subItemFactory
            .Setup(subItemFactory => subItemFactory.ToModel(It.Is<ContentfulReference>(contentfulRef => contentfulRef.Slug.Equals("article-slug"))))
            .Returns(new SubItem("slug",
                                "title",
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                DateTime.MinValue,
                                DateTime.MaxValue,
                                string.Empty,
                                new(),
                                EColourScheme.Teal,
                                new List<string>()));

        // Act
        Topic result = _parentTopicFactory.ToModel(contentfulArticle);

        // Assert
        Assert.IsType<Topic>(result);
        Assert.Equal(2, result.SubItems.Count());
        Assert.Equal("title", result.SubItems.First().Title);
        Assert.Equal("slug", result.SubItems.First().Slug);
        Assert.Equal("title", result.SubItems.ToList()[1].Title);
        Assert.Equal("slug", result.SubItems.ToList()[1].Slug);
    }
}