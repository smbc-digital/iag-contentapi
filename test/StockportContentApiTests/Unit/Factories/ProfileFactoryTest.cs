using System.IO;
using System.Linq;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class ProfileFactoryTest : TestingBaseClass
    {
        private readonly IFactory<Profile> _profileFactory;

        public ProfileFactoryTest()
        {
            _profileFactory = new ProfileFactory(new BreadcrumbFactory());
        }

        [Fact]
        public void BuildsProfileFromContentfulResponseData()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Profile.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var profile = (Profile)_profileFactory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            profile.Type.Should().Be("Success Story");
            profile.Title.Should().Be("Test Profile");
            profile.Slug.Should().Be("test-profile");
            profile.Subtitle.Should().Be("This is a test profile");
            profile.Body.Should().Be("Lots of test body content");
            profile.Icon.Should().Be("fa-icon");
            profile.BackgroundImage.Should().Be("image.jpg");
        }

        [Fact]
        public void BuildsProfileWithoutBackgroundImageFromContentfulResponseData()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.ProfileWithoutBackgroundImage.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var profile = (Profile)_profileFactory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);

            profile.Type.Should().Be("Success Story");
            profile.Title.Should().Be("Test Profile");
            profile.Slug.Should().Be("test-profile");
            profile.Subtitle.Should().Be("This is a test profile");
            profile.Body.Should().Be("Lots of test body content");
            profile.Icon.Should().Be("fa-icon");
            profile.BackgroundImage.Should().BeNullOrEmpty();
        }

        [Fact]
        public void BuildProfileWithBreadcrumbs()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.ProfileWithBreadcrumbs.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var profile = (Profile)_profileFactory.Build(contentfulResponse.GetFirstItem(),contentfulResponse);

            profile.Breadcrumbs.Should().HaveCount(2);
            profile.Breadcrumbs.First().Title.Should().Be("Test sub topic 1");
            profile.Breadcrumbs.First().Slug.Should().Be("test-sub-topic-1");
        }
    }
}
