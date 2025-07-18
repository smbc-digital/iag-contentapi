﻿namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ParentTopicContentfulFactoryTests
{
    private readonly ParentTopicContentfulFactory _parentTopicContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemContentfulFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();

    public ParentTopicContentfulFactoryTests()
    {
        _subitemContentfulFactory.Setup(subItem => subItem.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug", "title", "teaser", "teaser image", "icon", "type", DateTime.MinValue, DateTime.MaxValue,
                "image", new(), EColourScheme.Green));
        _timeProvider.Setup(timeProvider => timeProvider.Now())
            .Returns(new DateTime(2017, 01, 02));

        _parentTopicContentfulFactory = new(_subitemContentfulFactory.Object, _timeProvider.Object);
    }

    [Fact]
    public void ShouldReturnATopicFromAContentfulArticleBasedOnTheBreadcrumbs()
    {
        // Arrange
        List<ContentfulReference> subItemEntry = new()
        {
            new ContentfulReferenceBuilder().Slug("sub-slug").Build()
        };

        ContentfulReference ContentfulReferences = new ContentfulReferenceBuilder()
            .Title("test topic")
            .Slug("test-topic")
            .SubItems(subItemEntry)
            .SystemContentTypeId("topic")
            .Build();

        ContentfulArticle contentfulArticleEntry =
            new ContentfulArticleBuilder().Breadcrumbs(new() { ContentfulReferences }).Build();

        // Act
        Topic result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

        // Assert
        Assert.Equal("test topic", result.Title);
        Assert.Equal("test-topic", result.Slug);
        Assert.Single(result.SubItems);
    }

    [Fact]
    public void ShouldReturnNullTopicIfBreadcrumbDoesNotHaveTypeOfTopic()
    {
        // Arrange
        List<ContentfulReference> subItemEntry = new()
        {
            new ContentfulReferenceBuilder().Slug("sub-slug").Build()
        };

        ContentfulReference ContentfulReferences = new ContentfulReferenceBuilder()
            .Title("test topic")
            .Slug("test-topic")
            .SubItems(subItemEntry)
            .SystemContentTypeId("id")
            .Build();

        ContentfulArticle contentfulArticleEntry =
            new ContentfulArticleBuilder().Breadcrumbs(new() { ContentfulReferences }).Build();

        // Act
        Topic result = _parentTopicContentfulFactory.ToModel(contentfulArticleEntry);

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
        Topic result = _parentTopicContentfulFactory.ToModel(contentfulArticle);

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

        ContentfulReference ContentfulReferences = new ContentfulReferenceBuilder()
            .Title("test topic")
            .Slug("test-topic")
            .SubItems(subItemEntryList)
            .SystemContentTypeId("topic")
            .Build();

        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder()
            .Breadcrumbs(new()
            {
                ContentfulReferences
            })
            .Title("title")
            .Slug("slug")
            .SystemId("same-id-as-article")
            .Build();

        _subitemContentfulFactory.Setup(o => o.ToModel(It.Is<ContentfulReference>(x => x.Slug.Equals("article-slug"))))
            .Returns(new SubItem("slug", "title", string.Empty, string.Empty, string.Empty, string.Empty, DateTime.MinValue,
                DateTime.MaxValue, string.Empty, new(), EColourScheme.Teal));

        // Act
        Topic result = _parentTopicContentfulFactory.ToModel(contentfulArticle);

        // Assert
        Assert.IsType<Topic>(result);
        Assert.Equal(2, result.SubItems.Count());
        Assert.Equal("title", result.SubItems.First().Title);
        Assert.Equal("slug", result.SubItems.First().Slug);
        Assert.Equal("title", result.SubItems.ToList()[1].Title);
        Assert.Equal("slug", result.SubItems.ToList()[1].Slug);
    }
}