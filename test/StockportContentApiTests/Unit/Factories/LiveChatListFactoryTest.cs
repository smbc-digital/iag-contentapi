using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using StockportContentApi;
using StockportContentApi.Factories;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Xunit;

namespace StockportContentApiTests.Unit.Factories
{
    public class LiveChatListFactoryTest
    {
        private readonly LiveChatListFactory _liveChatListFactory;

        public LiveChatListFactoryTest()
        {            
            _liveChatListFactory = new LiveChatListFactory(new LiveChatFactory());
        }

        [Fact]
        public void BuildsLiveChatListFromContentfulResponseData()
        {
            dynamic mockContentfulData =
                JsonConvert.DeserializeObject(
                    File.ReadAllText("Unit/MockContentfulResponses/LiveChat/ArticleWithOnlyLiveChat.json"));
            var contentfulResponse = new ContentfulResponse(mockContentfulData);

            var article = contentfulResponse.GetFirstItem();
            var liveChat = _liveChatListFactory.BuildFromReference(article.fields.liveChatText,
                contentfulResponse) as LiveChat;

            liveChat.Title.Should().Be("Live Chat");
            liveChat.Text.Should().Be("Live Chat Text");

        }

    }
}