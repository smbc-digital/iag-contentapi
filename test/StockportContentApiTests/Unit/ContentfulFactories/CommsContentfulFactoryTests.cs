namespace StockportContentApiTests.Unit.ContentfulFactories;

public class CommsContentfulFactoryTests
{
    private readonly Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> _callToActionFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulEvent, Event>> _eventFactory = new();
    private readonly CommsContentfulFactory _factory;

    public CommsContentfulFactoryTests() =>
        _factory = new CommsContentfulFactory(_callToActionFactory.Object, _eventFactory.Object);

    [Fact]
    public void ToModel_ShouldReturnLinkList()
    {
        // Arrange
        _callToActionFactory
            .Setup(callToActionFactory => callToActionFactory.ToModel(It.IsAny<ContentfulCallToActionBanner>()))
            .Returns(new CallToActionBanner());
        
        _eventFactory
            .Setup(eventFactory => eventFactory.ToModel(It.IsAny<ContentfulEvent>()))
            .Returns(new EventBuilder().Build());

        ContentfulCommsHomepage model = new()
        {
            WhatsOnInStockportEvent = new ContentfulEventBuilder().Build(),
            MetaDescription = "meta description",
            CallToActionBanner = new ContentfulCallToActionBanner(),
            UsefulLinksText = new() { "Test Link" },
            UsefulLinksURL = new() { "https://www.stockport.gov.uk/" },
            TwitterFeedHeader = "twiiter",
            InstagramFeedTitle = "instagram header",
            FacebookFeedTitle = "facebook",
            Title = "title",
            InstagramLink = "instagram link",
            LatestNewsHeader = "latest news",
            Sys = new SystemProperties()
        };

        // Act
        CommsHomepage result = _factory.ToModel(model);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.WhatsOnInStockportEvent);
        Assert.NotNull(result.CallToActionBanner);
        Assert.NotNull(result.UsefulLinks);
        Assert.Equal("twiiter", result.TwitterFeedHeader);
        Assert.Equal("meta description", result.MetaDescription);
        Assert.Equal("instagram header", result.InstagramFeedTitle);
        Assert.Equal("facebook", result.FacebookFeedTitle);
        Assert.Equal("title", result.Title);
        Assert.Equal("instagram link", result.InstagramLink);
        Assert.Equal("latest news", result.LatestNewsHeader);
    }
}