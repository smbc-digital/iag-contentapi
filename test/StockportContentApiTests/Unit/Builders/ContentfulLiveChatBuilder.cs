using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulLiveChatBuilder
    {
        private string _title = "title";
        private string _text = "text";
        private SystemProperties _sys = new SystemProperties { Type = "Entry" };

    public ContentfulLiveChat Build()
        {
            return new ContentfulLiveChat()
            {
                Title = _title,
                Text = _text,
                Sys = _sys
            };
        }
    }
}
