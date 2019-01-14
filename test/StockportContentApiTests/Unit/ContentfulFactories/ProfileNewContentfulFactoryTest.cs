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
    public class ProfileNewContentfulFactoryTest
    {
        private readonly ContentfulProfileNew _contentfulProfileNew;
        private readonly Mock<IContentfulFactory<ContentfulReference, Crumb>> _crumbFactory;
        private readonly ProfileNewContentfulFactory _profileNewContentfulFactory;

        public ProfileNewContentfulFactoryTest()
        {
            _contentfulProfileNew = new ContentfulProfileBuilder().BuildNew();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulReference, Crumb>>();
            _profileNewContentfulFactory = new ProfileNewContentfulFactory(_crumbFactory.Object, HttpContextFake.GetHttpContextFake(), new Mock<IContentfulFactory<ContentfulAlert, Alert>>().Object, new Mock<IContentfulFactory<ContentfulInformationList, InformationList>>().Object);
        }

        [Fact]
        public void ShouldNotAddBreadcrumbsOrAlertsOrImageIfTheyAreLinks()
        {
            _contentfulProfileNew.Image.SystemProperties.Type = "Link";
            _contentfulProfileNew.Breadcrumbs.First().Sys.Type = "Link";
            _contentfulProfileNew.Alerts.First().Sys.Type = "Link";

            var profile = _profileNewContentfulFactory.ToModel(_contentfulProfileNew);

            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulReference>()), Times.Never);
            profile.Breadcrumbs.Count().Should().Be(0);
            profile.Image.Should().BeEmpty();
            profile.Alerts.Should().BeEmpty();
        }
    }
}
