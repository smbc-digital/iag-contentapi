﻿namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ArticleContentfulFactoryTest
{
    private readonly ContentfulArticle _contentfulArticle;
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();
    private readonly ArticleContentfulFactory _articleFactory;
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulArticle, Topic>> _parentTopicFactory = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();

    public ArticleContentfulFactoryTest()
    {
        _contentfulArticle = new ContentfulArticleBuilder()
            .WithBreadcrumbContentType("topic")
            .Build();

        _timeProvider.Setup(_ => _.Now()).Returns(new DateTime(2017, 01, 01));

        _articleFactory = new(_sectionFactory.Object, _crumbFactory.Object, _profileFactory.Object,
            _parentTopicFactory.Object, _documentFactory.Object, _videoRepository.Object, _timeProvider.Object, _alertFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldNotAddLinks()
    {
        // Arrange
        _contentfulArticle.BackgroundImage.SystemProperties.LinkType = "Link";
        _contentfulArticle.Sections.First().Sys.LinkType = "Link";
        _contentfulArticle.Breadcrumbs.First().Sys.LinkType = "Link";
        _contentfulArticle.Alerts.First().Sys.LinkType = "Link";
        _contentfulArticle.Profiles.First().Sys.LinkType = "Link";
        _contentfulArticle.Documents.First().SystemProperties.LinkType = "Link";

        // Act
        var article = _articleFactory.ToModel(_contentfulArticle);

        // Assert
        Assert.Empty(article.BackgroundImage);
        Assert.Empty(article.Sections);
        Assert.Empty(article.Breadcrumbs);
        Assert.Empty(article.Alerts);
        Assert.Empty(article.Profiles);
        Assert.Equivalent(new NullTopic(), article.ParentTopic);
        Assert.Empty(article.Documents);

        _sectionFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulSection>()), Times.Never);
        _crumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _crumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _crumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _crumbFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldRemoveAlertsInlineThatArePastSunsetDateOrBeforeSunriseDate()
    {
        // Arrange
        ContentfulAlert _visibleAlert = new()
        {
            Title = "title",
            SubHeading = "subHeading",
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2016, 12, 01),
            SunsetDate = new DateTime(2017, 02, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        ContentfulAlert _invisibleAlert =
            new()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(2017, 05, 01),
                SunsetDate = new DateTime(2017, 07, 01),
                Sys = new SystemProperties() { Type = "Entry" }
            };

        var contentfulArticle = new ContentfulArticleBuilder()
            .AlertsInline(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert })
            .Build();

        // Act
        var article = _articleFactory.ToModel(contentfulArticle);

        // Assert
        Assert.Single(article.AlertsInline);
    }

    [Fact]
    public void ToModel_ShouldRemoveAlertsThatArePastSunsetDateOrBeforeSunriseDate()
    {
        // Arrange
        ContentfulAlert _visibleAlert = new()
        {
            Title = "title",
            SubHeading = "subHeading",
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2016, 12, 01),
            SunsetDate = new DateTime(2017, 02, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        ContentfulAlert _invisibleAlert = new()
        {
            Title = "title",
            SubHeading = "subHeading",
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2017, 05, 01),
            SunsetDate = new DateTime(2017, 07, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        var contentfulArticle = new ContentfulArticleBuilder()
            .Alerts(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert })
            .Build();

        // Act
        var article = _articleFactory.ToModel(contentfulArticle);

        // Arrange
        Assert.Single(article.Alerts);
    }

    [Fact]
    public void ToModel_ShouldParseArticleIfBodyIsNull()
    {
        // Arrange
        var contentfulArticle = new ContentfulArticleBuilder().Title("title").Body(null).Build();

        // Act
        var article = _articleFactory.ToModel(contentfulArticle);

        // Assert
        Assert.IsType<Article>(article);
        Assert.Empty(article.Body);
        Assert.Equal("title", article.Title);
    }

    [Fact]
    public void ToModel_ShouldUpdateLastUpdatedAtWhenArticleSectionIsUpdated()
    {
        // Arrange
        var time = DateTime.Now.AddHours(1);
        var contentfulSection = new ContentfulSectionBuilder().AddUpdatedAt(time).Build();
        var contentfulArticle = new ContentfulArticleBuilder().Title("title").WithOutSection().Section(contentfulSection).Build();
        
        // Act
        var model = _articleFactory.ToModel(contentfulArticle);

        // Assert
        Assert.Equal(time, model.UpdatedAt);
    }
}