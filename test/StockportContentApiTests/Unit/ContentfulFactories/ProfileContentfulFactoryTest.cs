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
        [Fact]
        public void ShouldCreateAProfileFromAContentfulProfile()
        {
            var contentfulProfile = new ContentfulProfileBuilder().Build();
            
            var crumb = new Crumb("title", "slug", "type");
            var crumbFactory = new Mock<IContentfulFactory<Entry<ContentfulCrumb>, Crumb>>();
            crumbFactory.Setup(o => o.ToModel(contentfulProfile.Breadcrumbs.First())).Returns(crumb);

            var profile = new ProfileContentfulFactory(crumbFactory.Object).ToModel(contentfulProfile);

            profile.ShouldBeEquivalentTo(contentfulProfile, o => o.Excluding(e => e.Breadcrumbs)
                                                    .Excluding(e => e.Image).Excluding(e => e.BackgroundImage));
            profile.Image.Should().Be(contentfulProfile.Image.File.Url);
            profile.BackgroundImage.Should().Be(contentfulProfile.BackgroundImage.File.Url);

            crumbFactory.Verify(o => o.ToModel(contentfulProfile.Breadcrumbs.First()), Times.Once);
            profile.Breadcrumbs.First().ShouldBeEquivalentTo(crumb);      
        }
    }
}
