namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ProfileContentfulFactoryTests
{
    private readonly ContentfulProfile _contentfulProfile;
    private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulTrivia, Trivia>> _triviaFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>> _inlineQuoteFactory = new();
    private readonly ProfileContentfulFactory _profileContentfulFactory;
    private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory = new();
    private readonly Mock<IContentfulFactory<ContentfulProfile, Topic>> _parentTopicFactory = new();

    public ProfileContentfulFactoryTests()
    {
        _contentfulProfile = new ContentfulProfileBuilder().Build();
        _profileContentfulFactory = new ProfileContentfulFactory(_crumbFactory.Object,
                                                                _alertFactory.Object,
                                                                _triviaFactory.Object,
                                                                _inlineQuoteFactory.Object,
                                                                _eventBannerFactory.Object,
                                                                _parentTopicFactory.Object);
    }

    [Fact]
    public void ToModel_ShouldNotAddLinks()
    {
        // Arrange
        _contentfulProfile.Breadcrumbs.First().Sys.LinkType = "Link";
        _contentfulProfile.Alerts.First().Sys.LinkType = "Link";
        _contentfulProfile.TriviaSection.First().Sys.LinkType = "Link";

        // Act
        Profile profile = _profileContentfulFactory.ToModel(_contentfulProfile);

        // Assert
        Assert.Empty(profile.Breadcrumbs);
        Assert.Empty(profile.Alerts);
        Assert.Empty(profile.TriviaSection);
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()), Times.Never);
        _triviaFactory.Verify(triviaFactory => triviaFactory.ToModel(It.IsAny<ContentfulTrivia>()), Times.Never);
    }

    [Fact]
    public void ToModel_ShouldReturnNull()
    {
        // Act
        Profile profile = _profileContentfulFactory.ToModel(null);

        // Assert
        Assert.Null(profile);
    }

    [Fact]
    public void ToModel_ShouldReturnProfile()
    {
        // Act
        Profile profile = _profileContentfulFactory.ToModel(_contentfulProfile);

        // Assert
        Assert.NotEmpty(profile.Image.Url);
        Assert.NotEmpty(profile.Breadcrumbs);
        Assert.NotEmpty(profile.Alerts);
        Assert.NotEmpty(profile.TriviaSection);
        _crumbFactory.Verify(crumbFactory => crumbFactory.ToModel(It.IsAny<ContentfulReference>()), Times.Once);
        _alertFactory.Verify(alertFactory => alertFactory.ToModel(It.IsAny<ContentfulAlert>()), Times.Once);
        _triviaFactory.Verify(triviaFactory => triviaFactory.ToModel(It.IsAny<ContentfulTrivia>()), Times.Once);
        _inlineQuoteFactory.Verify(inlineQuoteFactory => inlineQuoteFactory.ToModel(It.IsAny<ContentfulInlineQuote>()), Times.Once);
        _eventBannerFactory.Verify(eventBannerFactory => eventBannerFactory.ToModel(It.IsAny<ContentfulEventBanner>()), Times.Once);
    }
}