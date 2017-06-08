using System.Linq;
using Contentful.Core.Models;
using FluentAssertions;
using Moq;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApiTests.Unit.Builders;
using Xunit;

namespace StockportContentApiTests.Unit.ContentfulFactories
{
    public class ProfileContentfulFactoryTest
    {
        private readonly ContentfulProfile _contentfulProfile;
        private readonly Mock<IContentfulFactory<ContentfulCrumb, Crumb>> _crumbFactory;
        private readonly ProfileContentfulFactory _profileContentfulFactory;

        public ProfileContentfulFactoryTest()
        {
            _contentfulProfile = new ContentfulProfileBuilder().Build();
            _crumbFactory = new Mock<IContentfulFactory<ContentfulCrumb, Crumb>>();
            _profileContentfulFactory = new ProfileContentfulFactory(_crumbFactory.Object);
        }

        [Fact]
        public void ShouldCreateAProfileFromAContentfulProfile()
        {
            var crumb = new Crumb("title", "slug", "type");
            _crumbFactory.Setup(o => o.ToModel(_contentfulProfile.Breadcrumbs.First())).Returns(crumb);

            var profile = _profileContentfulFactory.ToModel(_contentfulProfile);

            profile.ShouldBeEquivalentTo(_contentfulProfile, o => o.Excluding(e => e.Breadcrumbs)
                .Excluding(e => e.Image).Excluding(e => e.BackgroundImage));
            profile.Image.Should().Be(_contentfulProfile.Image.File.Url);
            profile.BackgroundImage.Should().Be(_contentfulProfile.BackgroundImage.File.Url);
            _crumbFactory.Verify(o => o.ToModel(_contentfulProfile.Breadcrumbs.First()), Times.Once);
            profile.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);
        }

        [Fact]
        public void ShouldNotAddBreadcrumbsOrImageIfTheyAreLinks()
        {
            _contentfulProfile.Image.SystemProperties.Type = "Link";
            _contentfulProfile.BackgroundImage.SystemProperties.Type = "Link";
            _contentfulProfile.Breadcrumbs.First().Sys.Type = "Link";

            var profile = _profileContentfulFactory.ToModel(_contentfulProfile);

            _crumbFactory.Verify(o => o.ToModel(It.IsAny<ContentfulCrumb>()), Times.Never);
            profile.Breadcrumbs.Count().Should().Be(0);
            profile.BackgroundImage.Should().BeEmpty();
            profile.Image.Should().BeEmpty();
        }
    }
}
