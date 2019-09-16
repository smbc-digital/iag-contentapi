using System.Linq;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;
using StockportContentApi.Fakes;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ProfileContentfulFactoryTest
    {
        private readonly ContentfulProfile _contentfulProfile;
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
        private readonly ProfileContentfulFactory _profileContentfulFactory;
        private readonly Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>> _inlineQuoteContentfulFactory;
        private readonly Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>> _eventBannerFactory;

        public ProfileContentfulFactoryTest()
        {
            _contentfulProfile = new ContentfulProfileBuilder().Build();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _eventBannerFactory = new Mock<IContentfulFactory<ContentfulEventBanner, EventBanner>>();
            _profileContentfulFactory = new ProfileContentfulFactory(
                _crumbFactory.Object, 
                HttpContextFake.GetHttpContextFake(), 
                new Mock<IContentfulFactory<ContentfulAlert, Alert>>().Object, 
                new Mock<IContentfulFactory<ContentfulInformationList, InformationList>>().Object,
                new Mock<IContentfulFactory<ContentfulInlineQuote, InlineQuote>>().Object,
                _eventBannerFactory.Object);
        }

        [Fact]
        public void ShouldNotAddBreadcrumbsOrAlertsOrImageIfTheyAreLinks()
        {
            _contentfulProfile.Image.SystemProperties.Type = "Link";
            _contentfulProfile.Breadcrumbs.First().Sys.Type = "Link";
            _contentfulProfile.Alerts.First().Sys.Type = "Link";

            var profile = _profileContentfulFactory.ToModel(_contentfulProfile);

            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
            profile.Breadcrumbs.Count().Should().Be(0);
            profile.Image.Should().BeEmpty();
            profile.Alerts.Should().BeEmpty();
        }
    }
}
