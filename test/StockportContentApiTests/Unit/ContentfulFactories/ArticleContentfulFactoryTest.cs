namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ArticleContentfulFactoryTest
{
    private readonly ContentfulArticle _contentfulArticle;
    private readonly Mock<IVideoRepository> _videoRepository;
    private readonly Mock<IContentfulFactory<ContentfulSection, Section>> _sectionFactory;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
    private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory;
    private readonly ArticleContentfulFactory _articleFactory;
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory;
    private readonly Mock<IContentfulFactory<ContentfulArticle, Topic>> _parentTopicFactory;
    private readonly Mock<ITimeProvider> _timeProvider;
    private Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

    public ArticleContentfulFactoryTest()
    {
        _contentfulArticle = new ContentfulArticleBuilder().Build();

        // set to topic for mocking
        // TODO: Refactor into builder
        _contentfulArticle.Breadcrumbs[0].Sys.ContentType.SystemProperties.Id = "topic";

        _videoRepository = new Mock<IVideoRepository>();
        _sectionFactory = new Mock<IContentfulFactory<ContentfulSection, Section>>();
        _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
        _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();
        _documentFactory = new Mock<IContentfulFactory<Asset, Document>>();
        _parentTopicFactory = new Mock<IContentfulFactory<ContentfulArticle, Topic>>();
        _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();

        _timeProvider = new Mock<ITimeProvider>();

        _timeProvider.Setup(o => o.Now()).Returns(new DateTime(2017, 01, 01));

        _articleFactory = new ArticleContentfulFactory(_sectionFactory.Object, _crumbFactory.Object, _profileFactory.Object,
            _parentTopicFactory.Object, _documentFactory.Object, _videoRepository.Object, _timeProvider.Object, _alertFactory.Object);
    }

    [Fact]
    public void ShouldNotAddBackgroundImageOrSectionsOrBreadcrumbsOrAlertsOrProfilesOrParentTopicOrDocumentsOrLiveChatIfTheyAreLinks()
    {
        _contentfulArticle.BackgroundImage.SystemProperties.LinkType = "Link";
        _contentfulArticle.Sections.First().Sys.LinkType = "Link";
        _contentfulArticle.Breadcrumbs.First().Sys.LinkType = "Link";
        _contentfulArticle.Alerts.First().Sys.LinkType = "Link";
        _contentfulArticle.Profiles.First().Sys.LinkType = "Link";
        _contentfulArticle.Documents.First().SystemProperties.LinkType = "Link";

        var article = _articleFactory.ToModel(_contentfulArticle);

        article.BackgroundImage.Should().BeEmpty();

        article.Sections.Count.Should().Be(0);
        _sectionFactory.Verify(o => o.ToModel(It.IsAny<ContentfulSection>()), Times.Never);

        article.Breadcrumbs.Count().Should().Be(0);
        _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);

        article.Alerts.Count().Should().Be(0);
        _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);

        article.Profiles.Count().Should().Be(0);
        _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);

        article.ParentTopic.Should().BeEquivalentTo(new NullTopic());

        article.Documents.Count.Should().Be(0);
        _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
    }

    [Fact]
    public void ItShouldRemoveAlertsInlineThatArePastSunsetDateOrBeforeSunriseDate()
    {
        ContentfulAlert _visibleAlert = new ContentfulAlert()
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
            new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(2017, 05, 01),
                SunsetDate = new DateTime(2017, 07, 01),
                Sys = new SystemProperties() { Type = "Entry" }
            };

        var contentfulArticle =
            new ContentfulArticleBuilder().AlertsInline(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert })
                .Build();

        var article = _articleFactory.ToModel(contentfulArticle);

        article.AlertsInline.Count().Should().Be(1);
    }

    [Fact]
    public void ItShouldRemoveAlertsThatArePastSunsetDateOrBeforeSunriseDate()
    {


        ContentfulAlert _visibleAlert = new ContentfulAlert()
        {
            Title = "title",
            SubHeading = "subHeading",
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2016, 12, 01),
            SunsetDate = new DateTime(2017, 02, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };
        ContentfulAlert _invisibleAlert = new ContentfulAlert()
        {
            Title = "title",
            SubHeading = "subHeading",
            Body = "body",
            Severity = "severity",
            SunriseDate = new DateTime(2017, 05, 01),
            SunsetDate = new DateTime(2017, 07, 01),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        var contentfulArticle =
            new ContentfulArticleBuilder().Alerts(new List<ContentfulAlert> { _visibleAlert, _invisibleAlert }).Build();

        var article = _articleFactory.ToModel(contentfulArticle);

        article.Alerts.Count().Should().Be(1);
    }

    [Fact]
    public void ShouldParseArticleIfBodyIsNull()
    {
        var contentfulArticle = new ContentfulArticleBuilder().Title("title").Body(null).Build();

        var article = _articleFactory.ToModel(contentfulArticle);

        article.Should().BeOfType<Article>();
        article.Body.Should().Be(string.Empty);
        article.Title.Should().Be("title");
    }

    [Fact]
    public void ToModelShouldUpdateLastUpdatedAtWhenArticleSectionisUpdated()
    {
        var time = DateTime.Now.AddHours(1);
        var contentfulSection = new ContentfulSectionBuilder().AddUpdatedAt(time).Build();
        var contentfulArticle = new ContentfulArticleBuilder().Title("title").WithOutSection().Section(contentfulSection).Build();

        var model = _articleFactory.ToModel(contentfulArticle);

        model.UpdatedAt.Should().Be(time);
    }
}

