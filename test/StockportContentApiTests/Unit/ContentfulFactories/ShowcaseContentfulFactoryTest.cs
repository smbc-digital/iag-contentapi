namespace StockportContentApiTests.Unit.ContentfulFactories;

public class ShowcaseContentfulFactoryTest
{
    [Fact]
    public void ShouldCreateAShowcaseFromAContentfulShowcase()
    {
        List<SubItem> subItems = new() {
            new("slug", "title", "teaser", "icon", "type", "contentType", DateTime.MinValue, DateTime.MaxValue, "image", "111", "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue, string.Empty, string.Empty) };
        Crumb crumb = new("title", "slug", "type");

        ContentfulShowcase contentfulShowcase = new ContentfulShowcaseBuilder()
            .Title("showcase title")
            .Slug("showcase-slug")
            .HeroImage(new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } })
            .Teaser("showcase teaser")
            .MetaDescription("showcase metaDescription")
            .Subheading("subheading")
            .SecondaryItems(new List<ContentfulReference>() { new() { Sys = new SystemProperties() { Type = "Entry" } } })
            .Build();

        Mock<IContentfulFactory<ContentfulReference, SubItem>> topicFactory = new();
        topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem("slug", "title", "teaser", "icon", "type", "contenType", DateTime.MinValue, DateTime.MaxValue, "image", "111", "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue, string.Empty, string.Empty));

        Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));

        Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();

        Mock<IContentfulFactory<ContentfulTrivia, Trivia>> _triviaFactory = new();

        Mock<IContentfulFactory<ContentfulReference, Crumb>> crumbFactory = new();
        crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

        Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> socialMediaFactory = new();
        socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

        Mock<IContentfulFactory<ContentfulTrivia, Trivia>> didYouKnowFactory = new();

        Mock<IContentfulFactory<ContentfulVideo, Video>> videoFactory = new();

        Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> callToActionBanner = new();
        callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
            new CallToActionBanner
            {
                Title = "title",
                AltText = "altText",
                ButtonText = "button text",
                Image = "url",
                Link = "url"
            });

        Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>> spotlightBannerFactory = new();

        Mock<ITimeProvider> timeprovider = new();

        timeprovider.Setup(_timeProvider => _timeProvider.Now()).Returns(new DateTime(2017, 03, 30));

        ShowcaseContentfulFactory contentfulFactory = new(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, videoFactory.Object, spotlightBannerFactory.Object);

        Showcase showcase = contentfulFactory.ToModel(contentfulShowcase);

        showcase.Should().BeOfType<Showcase>();
        showcase.Slug.Should().Be("showcase-slug");
        showcase.Title.Should().Be("showcase title");
        showcase.HeroImageUrl.Should().Be("image-url.jpg");
        showcase.Teaser.Should().Be("showcase teaser");
        showcase.MetaDescription.Should().Be("showcase metaDescription");
        showcase.Subheading.Should().Be("subheading");
        showcase.SecondaryItems.First().Title.Should().Be(subItems.First().Title);
        showcase.SecondaryItems.First().Icon.Should().Be(subItems.First().Icon);
        showcase.SecondaryItems.First().Slug.Should().Be(subItems.First().Slug);
        showcase.SecondaryItems.Should().HaveCount(1);
        showcase.Alerts.Count().Should().Be(1);
    }

    [Fact]
    public void ShouldCreateAShowcaseWithAnEmptyFeaturedItems()
    {
        ContentfulShowcase contentfulShowcase = new ContentfulShowcaseBuilder().SecondaryItems(new List<ContentfulReference>()).Build();

        Mock<IContentfulFactory<ContentfulReference, SubItem>> topicFactory = new();
        topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug", "title", "teaser", "icon", "type", "contentType", DateTime.MinValue, DateTime.MaxValue, "image", "111", "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue, string.Empty, string.Empty));

        Mock<IContentfulFactory<ContentfulReference, Crumb>> crumbFactory = new();

        Mock<ITimeProvider> timeprovider = new();

        timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

        Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> socialMediaFactory = new();
        socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

        Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));

        Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();

        Mock<IContentfulFactory<ContentfulTrivia, Trivia>> _triviaFactory = new();

        Mock<IContentfulFactory<ContentfulVideo, Video>> _videoFactory = new();

        Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> callToActionBanner = new();
        callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
            new CallToActionBanner
            {
                Title = "title",
                AltText = "altText",
                ButtonText = "button text",
                Image = "url",
                Link = "url"
            });

        Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>> spotlightBannerFactory = new();

        ShowcaseContentfulFactory contentfulFactory = new(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

        Showcase model = contentfulFactory.ToModel(contentfulShowcase);

        model.Should().BeOfType<Showcase>();
        model.SecondaryItems.Should().BeEmpty();
    }

    [Fact]
    public void ShouldCreateAShowcaseWithoutExpiredSubItems()
    {
        ContentfulReference subItemThatShouldDisplay = new()
        {

            Title = "Sub1",
            SunriseDate = new DateTime(2016, 12, 30),
            SunsetDate = new DateTime(2018, 12, 30),
            Sys = new SystemProperties() { Type = "Entry" }

        };

        ContentfulReference subItemThatShouldHaveExpired = new()
        {
            Title = "Sub1",
            SunriseDate = new DateTime(2016, 12, 30),
            SunsetDate = new DateTime(2017, 01, 30),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        List<ContentfulReference> subItems = new() { subItemThatShouldDisplay, subItemThatShouldHaveExpired };

        ContentfulShowcase contentfulShowcase = new ContentfulShowcaseBuilder().SecondaryItems(subItems).Build();

        Mock<IContentfulFactory<ContentfulReference, SubItem>> topicFactory = new();
        topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug", "title", "teaser", "icon", "type", "contentType", DateTime.MinValue, DateTime.MaxValue, "image", "111", "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue, string.Empty, string.Empty));

        Mock<IContentfulFactory<ContentfulReference, Crumb>> crumbFactory = new();

        Mock<ITimeProvider> timeprovider = new();

        timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

        Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> socialMediaFactory = new();
        socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

        Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));

        Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();

        Mock<IContentfulFactory<ContentfulTrivia, Trivia>> _triviaFactory = new();

        Mock<IContentfulFactory<ContentfulVideo, Video>> _videoFactory = new();

        Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> callToActionBanner = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();
        callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
            new CallToActionBanner
            {
                Title = "title",
                AltText = "altText",
                ButtonText = "button text",
                Image = "url",
                Link = "url"
            });

        Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>> spotlightBannerFactory = new();

        ShowcaseContentfulFactory contentfulFactory = new(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

        Showcase model = contentfulFactory.ToModel(contentfulShowcase);

        model.Should().BeOfType<Showcase>();
        model.SecondaryItems.Count().Should().Be(1);
    }

    [Fact]
    public void ShouldCreateAShowcaseWithoutSubItemsThatHaveNotArisen()
    {
        ContentfulReference subItemThatShouldNotYetDisplay = new()
        {
            Title = "Sub1",
            SunriseDate = new DateTime(2018, 12, 30),
            SunsetDate = new DateTime(2019, 12, 30),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        ContentfulReference subItemThatShouldDisplay = new()
        {
            Title = "Sub1",
            SunriseDate = new DateTime(2016, 12, 30),
            SunsetDate = new DateTime(2018, 01, 30),
            Sys = new SystemProperties() { Type = "Entry" }
        };

        List<ContentfulReference> subItems = new()
        {
            subItemThatShouldNotYetDisplay,
            subItemThatShouldDisplay
        };

        ContentfulShowcase contentfulShowcase = new ContentfulShowcaseBuilder().SecondaryItems(subItems).Build();

        Mock<IContentfulFactory<ContentfulReference, SubItem>> topicFactory = new();
        topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
            .Returns(new SubItem("slug", "title", "teaser", "icon", "type", "contentType", DateTime.MinValue, DateTime.MaxValue, "image", "111", "body text", new List<SubItem>(), "externalLink", "button text", EColourScheme.Blue, string.Empty, string.Empty));

        Mock<IContentfulFactory<ContentfulReference, Crumb>> crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();

        Mock<ITimeProvider> timeprovider = new();

        timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

        Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>> socialMediaFactory = new();
        socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

        Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory = new();
        _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false, string.Empty));

        Mock<IContentfulFactory<ContentfulProfile, Profile>> _profileFactory = new();

        Mock<IContentfulFactory<ContentfulTrivia, Trivia>> _triviaFactory = new();

        Mock<IContentfulFactory<ContentfulVideo, Video>> _videoFactory = new();

        Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>> callToActionBanner = new();
        callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
            new CallToActionBanner
            {
                Title = "title",
                AltText = "altText",
                ButtonText = "button text",
                Image = "url",
                Link = "url"
            });

        Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>> spotlightBannerFactory = new();

        ShowcaseContentfulFactory contentfulFactory = new(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

        Showcase model = contentfulFactory.ToModel(contentfulShowcase);

        model.Should().BeOfType<Showcase>();
        model.SecondaryItems.Count().Should().Be(1);
    }
}