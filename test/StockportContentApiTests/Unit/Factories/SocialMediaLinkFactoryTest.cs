using System;
using System.Collections.Generic;
using System.IO;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class SocialMediaLinkFactoryTest
    {
        private readonly IFactory<SocialMediaLink> _factory;

        public SocialMediaLinkFactoryTest()
        {
            _factory = new SocialMediaLinkFactory();
        }

        [Fact]
        public void BuildsSocialMediaLink()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/SocialMediaLinks.json"));   
            var contentfulResponse = new ContentfulResponse(mockContentfulData);
            
            var footer = (SocialMediaLink) _factory.Build(contentfulResponse.GetFirstItem(), contentfulResponse);
            footer.Title.Should().Be("Twitter title");
            footer.Url.Should().Be("https://twitter.com/test");
            footer.Slug.Should().Be("twitter-slug");
            footer.Icon.Should().Be("fa-twitter");
        }

    }
}
