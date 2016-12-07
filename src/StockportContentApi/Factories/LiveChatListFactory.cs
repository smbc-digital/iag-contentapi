using System.Collections.Generic;
using System.Linq;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class LiveChatListFactory : IBuildContentTypeFromReference<LiveChat>
    {
        private readonly IFactory<LiveChat> _liveChatFactory;

        public LiveChatListFactory(IFactory<LiveChat> liveChatFactory)
        {
            _liveChatFactory = liveChatFactory;
        }

        public LiveChat BuildFromReference(dynamic reference,
            IContentfulIncludes contentfulResponse)
        {
            if (reference == null) return new  NullLiveChat();;
            var liveChatEntry = contentfulResponse.GetEntryFor(reference);

            if (liveChatEntry == null) return new NullLiveChat();
            return _liveChatFactory.Build(liveChatEntry, contentfulResponse);
        }
    }
}
