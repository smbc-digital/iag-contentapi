using System;
using System.IO;
using FluentAssertions;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class LiveChatFactoryTest
    {
        private readonly IFactory<LiveChat> _liveChatFactory;

        public LiveChatFactoryTest()
        {
            _liveChatFactory = new LiveChatFactory();
        }

        [Fact]
        public void BuildsALiveChat()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/LiveChat/LiveChat.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
           LiveChat liveChat = _liveChatFactory.Build(entry, contentfulResponse);

            liveChat.Title.Should().Be("Live Chat");
            liveChat.Text.Should().Be("Live Chat Text");
        }

        [Fact]
        public void BuildAnEmptyLiveChatIfNoContent()
        {
            dynamic mockContentfulData = JsonConvert.DeserializeObject(File.ReadAllText("Unit/MockContentfulResponses/ContentNotFound.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var entry = contentfulResponse.GetFirstItem();
            LiveChat alert = _liveChatFactory.Build(entry, contentfulResponse);

            alert.Should().BeOfType<NullLiveChat>();
        }
    }
}
