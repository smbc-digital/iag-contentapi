using Contentful.Core.Configuration;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    public class LiveChat
    {
        public string Title { get; }
        public string Text { get; }      

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
