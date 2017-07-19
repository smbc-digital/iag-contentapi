using System.Collections.Generic;
using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class LiveChatContentfulFactory : IContentfulFactory<ContentfulLiveChat, LiveChat>
    {
        public LiveChat ToModel(ContentfulLiveChat entry)
        {
            return new LiveChat(entry.Title, entry.Text);
        }
    }
}