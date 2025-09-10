namespace StockportContentApiTests.Unit.ContentfulFactories;

public class NewsContentfulFactoryTests
{
    private readonly Mock<IVideoRepository> _videoRepository = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertBuilder = new();
    private readonly Mock<ITimeProvider> _timeProvider = new();
    private readonly NewsContentfulFactory _newsContentfulFactory;
    private readonly ContentfulNews _contentfulNews;
    private readonly Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>> _inlineQuoteFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulTrustedLogo, TrustedLogo>> _brandingFactory = new();

    public NewsContentfulFactoryTests()
    {
        _contentfulNews = new ContentfulNewsBuilder().Build();

        _newsContentfulFactory = new(_videoRepository.Object,
                                    _alertBuilder.Object,
                                    _timeProvider.Object,
                                    _inlineQuoteFactory.Object,
                                    _callToActionFactory.Object,
                                    _brandingFactory.Object);
    }

    [Fact]
    public void ShouldNotAddImageIfTheyAreLinks()
    {
        // Arrange
        _contentfulNews.Image.SystemProperties.LinkType = "Link";

        _videoRepository
            .Setup(videoRepo => videoRepo.Process(_contentfulNews.Body))
            .Returns(_contentfulNews.Body);

        // Act
        News news = _newsContentfulFactory.ToModel(_contentfulNews);

        // Assert
        Assert.Empty(news.Image);
        Assert.Empty(news.ThumbnailImage);
    }
}