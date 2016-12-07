using System;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class LiveChatFactory : IFactory<LiveChat>
    {
        public LiveChat Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null) return new NullLiveChat();
            var title = (string)entry.fields.title ?? string.Empty;
            var text = (string)entry.fields.text ?? string.Empty;
            return new LiveChat(title,text);
       }
    }
}
