using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class EventListContentfulFactory : IContentfulFactory<List<ContentfulEvent>, List<Event>>
    {
        private readonly IContentfulFactory<ContentfulEvent, Event> _eventFactory;

        public EventListContentfulFactory(IContentfulFactory<ContentfulEvent, Event> eventFactory)
        {
            _eventFactory = eventFactory;
        }

        public List<Event> ToModel(List<ContentfulEvent> entries)
        {
            var result = new List<Event>();
            foreach (var item in entries)
            {
                var model = _eventFactory.ToModel(item);
                result.Add(model);
            }

            return result;
        }
    }
}