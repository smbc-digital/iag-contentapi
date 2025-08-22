namespace StockportContentApiTests.Unit.ContentfulFactories;

public class SectionContentfulFactoryTests
{
    private readonly ContentfulSection _contentfulSection;
    private readonly Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();
    private readonly Mock<IContentfulFactory<Asset, Document>> _documentFactory = new();
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly SectionContentfulFactory _sectionFactory;
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _brandingFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>> _inlineQuoteContentfulFactory = new();

    public SectionContentfulFactoryTests()
    {
        _contentfulSection = new ContentfulSectionBuilder().Build();

        _timeProvider
            .Setup(time => time.Now())
            .Returns(new DateTime(2017, 01, 01));

        _sectionFactory = new SectionContentfulFactory(_profileFactory.Object,
                                                    _documentFactory.Object,
                                                    _videoRepository.Object,
                                                    _timeProvider.Object,
                                                    _alertFactory.Object,
                                                    _brandingFactory.Object,
                                                    _inlineQuoteContentfulFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldCreateASectionFromAContentfulSection()
    {
        // Arrange
        Profile profile = new()
        {
            Title = "title",
            Slug = "slug",
            Subtitle = "subtitle",
            Teaser = "teaser",
            Image = new MediaAsset(),
            ImageCaption = "imageCaption",
            Body = "body",
            Breadcrumbs = new List<Crumb>
            {
                new("title", "slug", "type")
            },
            Alerts = new List<Alert>
            {
                new AlertBuilder().Build()
            },
            TriviaSubheading = "trivia heading",
            TriviaSection = new List<Trivia>(),
            InlineQuotes = new List<InlineQuote>(),
            Author = "author",
            Subject = "subject",
            Colour = EColourScheme.Teal
        };

        _profileFactory
            .Setup(profileFactory => profileFactory.ToModel(_contentfulSection.Profiles.First()))
            .Returns(profile);

        Document document = new DocumentBuilder().Build();
        _documentFactory
            .Setup(documentFactory => documentFactory.ToModel(_contentfulSection.Documents.First()))
            .Returns(document);

        _videoRepository
            .Setup(videoFactory => videoFactory.Process(_contentfulSection.Body))
            .Returns("this is processed body");

        Alert alert = new("title", "body", "severity", DateTime.MinValue, DateTime.MinValue, "slug", false, string.Empty);
        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()))
            .Returns(alert);

        // Act
        Section result = _sectionFactory.ToModel(_contentfulSection);

        // Assert
        Assert.Single(result.AlertsInline);
        Assert.Equal(alert, result.AlertsInline.First());
        Assert.Equal("this is processed body", result.Body);
        Assert.Single(result.Documents);
        Assert.Equal(document, result.Documents.First());
        Assert.Single(result.Profiles);
        Assert.Equal(profile, result.Profiles.First());
        Assert.Equal("slug", result.Slug);
        Assert.Equal("title", result.Title);
    }

    [Fact]
    public void ToModel_ShouldNotAddDocumentsOrProfilesIfTheyAreLinks()
    {
        // Arrange
        _contentfulSection.Documents.First().SystemProperties.LinkType = "Link";
        _contentfulSection.Profiles.First().Sys.LinkType = "Link";
        _videoRepository
            .Setup(videoRepository => videoRepository.Process(_contentfulSection.Body))
            .Returns(_contentfulSection.Body);

        // Act
        Section section = _sectionFactory.ToModel(_contentfulSection);

        // Assert
        _documentFactory.Verify(documentFactory => documentFactory.ToModel(It.IsAny<Asset>()), Times.Never);
        _profileFactory.Verify(profileFactory => profileFactory.ToModel(It.IsAny<ContentfulProfile>()), Times.Never);
        Assert.Empty(section.Documents);
        Assert.Empty(section.Profiles);
    }
}