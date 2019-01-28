using System.Collections.Generic;
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
        private readonly Mock<IContentfulFactory<ContentfulInformationList, InformationList>> _informationListFactory;
        private readonly Mock<IContentfulFactory<ContentfulAlert, Alert>> _alertFactory;

        public ProfileContentfulFactoryTest()
        {
            _contentfulProfile = new ContentfulProfileBuilder().Build();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _informationListFactory = new Mock<IContentfulFactory<ContentfulInformationList, InformationList>>();
            _alertFactory = new Mock<IContentfulFactory<ContentfulAlert, Alert>>();
            _profileContentfulFactory = new ProfileContentfulFactory(_crumbFactory.Object, HttpContextFake.GetHttpContextFake(), _alertFactory.Object, _informationListFactory.Object);
        }

        [Fact]
        public void ShouldCreateAProfileFromAContentfulProfile()
        {
            var crumb = new Crumb("title", "slug", "type");
            _crumbFactory.Setup(o => o.ToModel(_contentfulProfile.Breadcrumbs.First())).Returns(crumb);

            var alert = new Alert(
                _contentfulProfile.Alerts[0].Title, 
                _contentfulProfile.Alerts[0].SubHeading,
                _contentfulProfile.Alerts[0].Body, 
                _contentfulProfile.Alerts[0].Severity,
                _contentfulProfile.Alerts[0].SunriseDate, 
                _contentfulProfile.Alerts[0].SunsetDate,
                _contentfulProfile.Alerts[0].Slug);

            _alertFactory
                .Setup(o => o.ToModel(_contentfulProfile.Alerts[0]))
                .Returns(alert);

            var profile = _profileContentfulFactory.ToModel(_contentfulProfile);

            profile.Should().BeEquivalentTo(_contentfulProfile, o => o.ExcludingMissingMembers());
            profile.Image.Should().Be(_contentfulProfile.Image.File.Url);
            _crumbFactory.Verify(o => o.ToModel(_contentfulProfile.Breadcrumbs.First()), Times.Once);
            profile.Breadcrumbs.First().Should().BeEquivalentTo(crumb);
        }

        [Fact]
        public void ShouldNotAddBreadcrumbsOrImageIfTheyAreLinks()
        {
            _contentfulProfile.Image.SystemProperties.Type = "Link";
            _contentfulProfile.Breadcrumbs.First().Sys.Type = "Link";

            var profile = _profileContentfulFactory.ToModel(_contentfulProfile);

            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
            profile.Breadcrumbs.Count().Should().Be(0);
            profile.Image.Should().BeEmpty();
        }
    }
}
