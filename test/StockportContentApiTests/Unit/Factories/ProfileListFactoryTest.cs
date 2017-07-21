using System.Collections.Generic;
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
    public class ProfileListFactoryTest : TestingBaseClass
    {
        private readonly IBuildContentTypesFromReferences<Profile> _profileListFactory;

        public ProfileListFactoryTest()
        {
            _profileListFactory = new ProfileListFactory();
        }

        [Fact]
        public void BuildsSubsetOfProfileDataWhenBuildingProfileListFromReferences()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(GetStringResponseFromFile("StockportContentApiTests.Unit.MockContentfulResponses.Article.ArticleWithMultipleProfiles.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();
            List<Profile> profiles = _profileListFactory.BuildFromReferences(article.fields.profiles, contentfulResponse);

            profiles.Count.Should().Be(2);
            var profile = profiles.First();
            profile.Type.Should().Be("Success Story");
            profile.Title.Should().Be("A profile");
            profile.Slug.Should().Be("test-profile");
            profile.Subtitle.Should().Be("This is a test profile");
            profile.Teaser.Should().Be("profile teaser");
            profile.Image.Should().Be("image.jpg");
            profile.Body.Should().BeNullOrEmpty();
            profile.Icon.Should().BeNullOrEmpty();
            profile.BackgroundImage.Should().BeNullOrEmpty();
            profile.Breadcrumbs.Should().BeNullOrEmpty();
        }

    }
}
