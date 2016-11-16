using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class SocialMediaLinkListFactoryTest
    {
        private readonly SocialMediaLinkListFactory _factory;
        private readonly ContentfulResponse _contentfulResponse;

        public SocialMediaLinkListFactoryTest()
        {
            _factory = new SocialMediaLinkListFactory(new SocialMediaLinkFactory());
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/FooterSocialMediaLinks.json"));
            _contentfulResponse = new ContentfulResponse(mockContentfulData);
        }

        [Fact]
        public void BuildsListOfSocialMediaLinkItems()
        {
            var socialMediaLinks = (IEnumerable<dynamic>)_contentfulResponse.Items;
            var items = _factory.BuildFromReferences(socialMediaLinks, _contentfulResponse);

            items.Should().HaveCount(4);
            items.First().Title.Should().Be("Twitter title");
            items.First().Slug.Should().Be("twitter-slug");
            items.First().Icon.Should().Be("fa-twitter");
            items.First().Url.Should().Be("https://twitter.com/test");

            items.Last().Title.Should().Be("RSS title");
            items.Last().Slug.Should().Be("rss-slug");
            items.Last().Icon.Should().Be("fa-rss");
            items.Last().Url.Should().Be("https://test.com");
        }
    }
}
