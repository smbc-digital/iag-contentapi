using TimeProvider = StockportContentApi.Utils.TimeProvider;

namespace StockportContentApiTests.Unit.ContentfulFactories;

public class PublicationSectionContentfulFactoryTests
{
    private readonly ITimeProvider _timeProvider = new TimeProvider();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>> _inlineQuoteFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _trustedLogoFactory = new();
    readonly ContentfulPublicationSection _contentfulPublicationSection = new ContentfulPublicationSectionBuilder().Build();
    private readonly PublicationSectionContentfulFactory _publicationSectionFactory;

    public PublicationSectionContentfulFactoryTests() =>
            _publicationSectionFactory = new PublicationSectionContentfulFactory(_timeProvider,
                                                                _alertFactory.Object,
                                                                _inlineQuoteFactory.Object,
                                                                _trustedLogoFactory.Object);

    [Fact]
    public void ToModel_ShouldCreateAPublicationSectionFromAContentfulPublicationSection()
    {
        // Arrange
        Alert alert = new("title", "body", "test", new DateTime(2017, 01, 01), new DateTime(2017, 04, 10), string.Empty, false, string.Empty);
        _contentfulPublicationSection.InlineAlerts = new List<ContentfulAlert>() { new ContentfulAlertBuilder().Build() };
        _contentfulPublicationSection.InlineQuotes = new List<ContentfulInlineQuote>() { new ContentfulInlineQuoteBuilder().Build() };

        _alertFactory
            .Setup(alertFactory => alertFactory.ToModel(_contentfulPublicationSection.InlineAlerts.First()))
            .Returns(alert);

        // Act
        PublicationSection result = _publicationSectionFactory.ToModel(_contentfulPublicationSection);

        // Assert
        Assert.Equal("slug", result.Slug);
        Assert.Equal("title", result.Title);
        Assert.Equal("meta description", result.MetaDescription);
        Assert.Equal(alert, result.InlineAlerts.First());
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(_contentfulPublicationSection.InlineAlerts.First()), Times.Once);
        _inlineQuoteFactory.Verify(inlineQuoteFactory => inlineQuoteFactory.ToModel(_contentfulPublicationSection.InlineQuotes.First()), Times.Once);
    }

    [Fact]
    public void ToModel_ShouldNotAddAlerts_If_TheyAreLinks()
    {
        // Arrange
        ContentfulPublicationSection contentfulPublicationSection = new ContentfulPublicationSectionBuilder().Build();

        contentfulPublicationSection.InlineAlerts.First().Sys.LinkType = "Link";

        // Act
        PublicationSection publicationSection = _publicationSectionFactory.ToModel(contentfulPublicationSection);

        // Assert
        Assert.Empty(publicationSection.InlineAlerts);
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(contentfulPublicationSection.InlineAlerts.First()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldReturnNull_IfNullEntry()
    {
        // Act & Assert
        Assert.Null(_publicationSectionFactory.ToModel(null));
    }
}