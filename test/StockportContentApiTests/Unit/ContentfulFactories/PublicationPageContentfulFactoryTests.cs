using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PublicationPageContentfulFactoryTests
{
    private readonly ITimeProvider _timeProvider = new TimeProvider();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>> _inlineQuoteFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _trustedLogoFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulPublicationSection, PublicationSection>> _publicationSectionFactory = new();
    readonly ContentfulPublicationPage _contentfulPublicationPage = new ContentfulPublicationPageBuilder().Build();
    private readonly PublicationPageContentfulFactory _publicationPageFactory;

    public PublicationPageContentfulFactoryTests() =>
            _publicationPageFactory = new PublicationPageContentfulFactory(_publicationSectionFactory.Object,
                                                                _timeProvider,
                                                                _alertFactory.Object,
                                                                _inlineQuoteFactory.Object,
                                                                _trustedLogoFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateAPublicationPageFromAContentfulPublicationPage()
    {
        // Arrange
        Alert alert = new("title", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10), string.Empty, false, string.Empty);
        _contentfulPublicationPage.InlineAlerts = new List<ContentfulAlert>() { new ContentfulAlertBuilder().Build() };
        _contentfulPublicationPage.InlineQuotes = new List<ContentfulInlineQuote>() { new ContentfulInlineQuoteBuilder().Build() };

        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(_contentfulPublicationPage.InlineAlerts.First()))
            .Returns(alert);

        // Act
        PublicationPage result = _publicationPageFactory.ToModel(_contentfulPublicationPage);

        // Assert
        Assert.Equal("slug", result.Slug);
        Assert.Equal("title", result.Title);
        Assert.Equal("meta description", result.MetaDescription);
        Assert.Equal(alert, result.InlineAlerts.First());
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(_contentfulPublicationPage.InlineAlerts.First()), Times.Once);
        _inlineQuoteFactory.Verify(inlineQuoteFactory => inlineQuoteFactory.ToModel(_contentfulPublicationPage.InlineQuotes.First()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldNotAddAlertsOrPublicationSections_If_TheyAreLinks()
    {
        // Arrange
        ContentfulPublicationPage contentfulPublicationPage = new ContentfulPublicationPageBuilder().Build();

        contentfulPublicationPage.InlineAlerts.First().Sys.LinkType = "Link";
        contentfulPublicationPage.PublicationSections.First().Sys.LinkType = "Link";

        // Act
        PublicationPage publicationPage = _publicationPageFactory.ToModel(contentfulPublicationPage);

        // Assert
        Assert.Empty(publicationPage.InlineAlerts);
        Assert.Empty(publicationPage.PublicationSections);
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(contentfulPublicationPage.InlineAlerts.First()), Times.Never);
        _publicationSectionFactory.Verify(publicationSectionFactory => publicationSectionFactory.ToModel(contentfulPublicationPage.PublicationSections.First()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act & Assert
        Assert.Null(_publicationPageFactory.ToModel(null));
    }
}