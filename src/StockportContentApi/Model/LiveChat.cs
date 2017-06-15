using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    public class LiveChat
    {
        public string Title { get; }
        public string Text { get; }
        public SystemProperties Sys { get; set; }

        public LiveChat(string title, string text)
        {
            Title = title;
            Text = text;
        }
    }

    public class NullLiveChat : LiveChat
    {
         public NullLiveChat() : base(string.Empty,string.Empty) { }
    }
}
