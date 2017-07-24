using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class EventHomepage
    {
        public List<string> Tags { get; }

        public EventHomepage(List<string> tags)
        {
            Tags = tags;
        }
    }
}
