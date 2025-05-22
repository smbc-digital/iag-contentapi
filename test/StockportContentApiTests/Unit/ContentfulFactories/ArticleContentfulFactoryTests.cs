namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ArticleContentfulFactoryTests
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
    private readonly Mock<IContentfulFactory<ContentfulReference, SubItem>> _subitemFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulGroupBranding, GroupBranding>> _articleBrandingFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>> _inlineQuoteFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();

    public ArticleContentfulFactoryTests()
    {
        _contentfulArticle = new ContentfulArticleBuilder()
            .WithBreadcrumbContentType("topic")
            .Build();

        _timeProvider
            .Setup(provider => provider.Now())
            .Returns(new DateTime(2017, 01, 01));

        _articleFactory = new(_sectionFactory.Object,
                              _crumbFactory.Object,
                              _profileFactory.Object,
                              _parentTopicFactory.Object,
                              _documentFactory.Object,
                              _videoRepository.Object,
                              _timeProvider.Object,
                              _alertFactory.Object,
                              _articleBrandingFactory.Object,
                              _subitemFactory.Object,
                              _inlineQuoteFactory.Object,
                              _callToActionFactory.Object);
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
        _contentfulArticle.Sys.CreatedAt = DateTime.Now;

        // Act
        Article article = _articleFactory.ToModel(_contentfulArticle);

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
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2016, 12, 01),
            SunsetDate = new DateTime(2017, 02, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        ContentfulAlert _invisibleAlert = new()
        {
            Title = "title",
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2017, 05, 01),
            SunsetDate = new DateTime(2017, 07, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder()
            .AlertsInline(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert })
            .Build();

        // Act
        Article article = _articleFactory.ToModel(contentfulArticle);

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
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2016, 12, 01),
            SunsetDate = new DateTime(2017, 02, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        ContentfulAlert _invisibleAlert = new()
        {
            Title = "title",
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2017, 05, 01),
            SunsetDate = new DateTime(2017, 07, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder()
            .Alerts(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert })
            .Build();

        // Act
        Article article = _articleFactory.ToModel(contentfulArticle);

        // Arrange
        Assert.Single(article.Alerts);
    }

    [Fact]
    public void ToModel_ShouldParseArticleIfBodyIsNull()
    {
        // Arrange
        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder().Title("title").Body(null).Build();

        // Act
        Article article = _articleFactory.ToModel(contentfulArticle);

        // Assert
        Assert.IsType<Article>(article);
        Assert.Empty(article.Body);
        Assert.Equal("title", article.Title);
    }

    [Fact]
    public void ToModel_ShouldCallBrandingFactory_WhenArticleBrandingIsNotNull()
    {
        // Arrange
        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder().Build();

        _articleBrandingFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulGroupBranding>()))
            .Returns(new GroupBranding("branding title", "branding text", new MediaAsset(), "branding-url"));

        // Act
        Article article = _articleFactory.ToModel(contentfulArticle);

        // Assert
        Assert.NotNull(article);
        Assert.Single(article.ArticleBranding);
        _articleBrandingFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulGroupBranding>()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldCallSubitemFactory_WhenRelatedContentIsNotNull()
    {
        // Arrange
        ContentfulArticle contentfulArticle = new ContentfulArticleBuilder().Build();

        _subitemFactory
            .Setup(factory => factory.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem());

        // Act
        Article article = _articleFactory.ToModel(contentfulArticle);

        // Assert
        Assert.NotNull(article);
        Assert.Single(article.RelatedContent);
        _subitemFactory.Verify(_ => _.ToModel(It.IsAny<ContentfulReference>()), Times.Once);
    }
}