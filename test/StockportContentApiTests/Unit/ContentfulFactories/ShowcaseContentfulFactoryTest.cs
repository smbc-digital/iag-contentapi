using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using StockportContentApiTests.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ShowcaseContentfulFactoryTest
    {
        [Fact]
        public void ShouldCreateAShowcaseFromAContentfulShowcase()
        {
            var subItems = new List<SubItem> {
                new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()) };
            var crumb = new Crumb("title", "slug", "type");

            var contentfulShowcase = new ContentfulShowcaseBuilder()
                .Title("showcase title")
                .Slug("showcase-slug")
                .HeroImage(new Asset { File = new File { Url = "image-url.jpg" }, SystemProperties = new SystemProperties { Type = "Asset" } })
                .Teaser("showcase teaser")
                .MetaDescription("showcase metaDescription")
                .Subheading("subheading")
                .SecondaryItems(new List<ContentfulReference>() { new ContentfulReference() { Sys = new SystemProperties() { Type = "Entry" } } })
                .Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false));


            var _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();

            var _triviaFactory = new Mock<IContentfulFactory<ContentfulTrivia, Trivia>>();

            var crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>())).Returns(crumb);

            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();
            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

            var didYouKnowFactory = new Mock<IContentfulFactory<ContentfulTrivia, Trivia>>();

            var videoFactory = new Mock<IContentfulFactory<ContentfulVideo, Video>>();

            var callToActionBanner = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();
            callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
                new CallToActionBanner
                {
                    Title = "title",
                    AltText = "altText",
                    ButtonText = "button text",
                    Image = "url",
                    Link = "url"
                });

            var spotlightBannerFactory = new Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>>();

            Mock<ITimeProvider> timeprovider = new Mock<ITimeProvider>();

            timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, videoFactory.Object, spotlightBannerFactory.Object);

            var showcase = contentfulFactory.ToModel(contentfulShowcase);

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
            var contentfulShowcase = new ContentfulShowcaseBuilder().SecondaryItems(new List<ContentfulReference>()).Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();

            var timeprovider = new Mock<ITimeProvider>();

            timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();
            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

            var _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false));

            var _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();

            var _triviaFactory = new Mock<IContentfulFactory<ContentfulTrivia, Trivia>>();

            var _videoFactory = new Mock<IContentfulFactory<ContentfulVideo, Video>>();

            var callToActionBanner = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();
            callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
                new CallToActionBanner
                {
                    Title = "title",
                    AltText = "altText",
                    ButtonText = "button text",
                    Image = "url",
                    Link = "url"
                });

            var spotlightBannerFactory = new Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>>();

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

            var model = contentfulFactory.ToModel(contentfulShowcase);

            model.Should().BeOfType<Showcase>();
            model.SecondaryItems.Should().BeEmpty();
        }

        [Fact]
        public void ShouldCreateAShowcaseWithoutExpiredSubItems()
        {
            var subItemThatShouldDisplay = new ContentfulReference()
            {

                Title = "Sub1",
                SunriseDate = new DateTime(2016, 12, 30),
                SunsetDate = new DateTime(2018, 12, 30),
                Sys = new SystemProperties() { Type = "Entry" }

            };

            var subItemThatShouldHaveExpired = new ContentfulReference()
            {
                Title = "Sub1",
                SunriseDate = new DateTime(2016, 12, 30),
                SunsetDate = new DateTime(2017, 01, 30),
                Sys = new SystemProperties() { Type = "Entry" }
            };

            var subItems = new List<ContentfulReference> { subItemThatShouldDisplay, subItemThatShouldHaveExpired };

            var contentfulShowcase = new ContentfulShowcaseBuilder().SecondaryItems(subItems).Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();

            var timeprovider = new Mock<ITimeProvider>();

            timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();
            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

            var _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false));

            var _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();

            var _triviaFactory = new Mock<IContentfulFactory<ContentfulTrivia, Trivia>>();

            var _videoFactory = new Mock<IContentfulFactory<ContentfulVideo, Video>>();

            var callToActionBanner = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();
            callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
                new CallToActionBanner
                {
                    Title = "title",
                    AltText = "altText",
                    ButtonText = "button text",
                    Image = "url",
                    Link = "url"
                });

            var spotlightBannerFactory = new Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>>();

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

            var model = contentfulFactory.ToModel(contentfulShowcase);

            model.Should().BeOfType<Showcase>();
            model.SecondaryItems.Count().Should().Be(1);
        }

        [Fact]
        public void ShouldCreateAShowcaseWithoutSubItemsThatHaveNotArisen()
        {
            var subItemThatShouldNotYetDisplay = new ContentfulReference()
            {
                Title = "Sub1",
                SunriseDate = new DateTime(2018, 12, 30),
                SunsetDate = new DateTime(2019, 12, 30),
                Sys = new SystemProperties() { Type = "Entry" }
            };

            var subItemThatShouldDisplay = new ContentfulReference()
            {
                Title = "Sub1",
                SunriseDate = new DateTime(2016, 12, 30),
                SunsetDate = new DateTime(2018, 01, 30),
                Sys = new SystemProperties() { Type = "Entry" }
            };

            var subItems = new List<ContentfulReference>();
            subItems.Add(subItemThatShouldNotYetDisplay);
            subItems.Add(subItemThatShouldDisplay);

            var contentfulShowcase = new ContentfulShowcaseBuilder().SecondaryItems(subItems).Build();

            var topicFactory = new Mock<IContentfulFactory<ContentfulReference, SubItem>>();
            topicFactory.Setup(o => o.ToModel(It.IsAny<ContentfulReference>()))
                .Returns(new SubItem("slug", "title", "teaser", "icon", "type", DateTime.MinValue, DateTime.MaxValue, "image", new List<SubItem>()));

            var crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();

            var timeprovider = new Mock<ITimeProvider>();

            timeprovider.Setup(o => o.Now()).Returns(new DateTime(2017, 03, 30));

            var socialMediaFactory = new Mock<IContentfulFactory<ContentfulSocialMediaLink, SocialMediaLink>>();
            socialMediaFactory.Setup(o => o.ToModel(It.IsAny<ContentfulSocialMediaLink>())).Returns(new SocialMediaLink("sm-link-title", "sm-link-slug", "sm-link-icon", "https://link.url", "sm-link-accountName", "sm-link-screenReader"));

            var _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _alertFactory.Setup(o => o.ToModel(It.IsAny<ContentfulAlert>())).Returns(new Alert("title", "", "", "", DateTime.MinValue, DateTime.MaxValue, string.Empty, false));

            var _profileFactory = new Mock<IContentfulFactory<ContentfulProfile, Profile>>();

            var _triviaFactory = new Mock<IContentfulFactory<ContentfulTrivia, Trivia>>();

            var _videoFactory = new Mock<IContentfulFactory<ContentfulVideo, Video>>();

            var callToActionBanner = new Mock<IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner>>();
            callToActionBanner.Setup(_ => _.ToModel(It.IsAny<ContentfulCallToActionBanner>())).Returns(
                new CallToActionBanner
                {
                    Title = "title",
                    AltText = "altText",
                    ButtonText = "button text",
                    Image = "url",
                    Link = "url"
                });

            var spotlightBannerFactory = new Mock<IContentfulFactory<ContentfulSpotlightBanner, SpotlightBanner>>();

            var contentfulFactory = new ShowcaseContentfulFactory(topicFactory.Object, crumbFactory.Object, timeprovider.Object, socialMediaFactory.Object, _alertFactory.Object, _profileFactory.Object, _triviaFactory.Object, callToActionBanner.Object, _videoFactory.Object, spotlightBannerFactory.Object);

            var model = contentfulFactory.ToModel(contentfulShowcase);

            model.Should().BeOfType<Showcase>();
            model.SecondaryItems.Count().Should().Be(1);
        }
    }
}
