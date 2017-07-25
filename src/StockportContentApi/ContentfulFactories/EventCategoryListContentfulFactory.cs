using System.Collections.Generic;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class EventCategoryListContentfulFactory : IContentfulFactory<IEnumerable<ContentfulEventCategory>, List<EventCategory>>
    {
        private readonly IContentfulFactory<ContentfulEventCategory, EventCategory> EventCategoryFactory;

        public EventCategoryListContentfulFactory(IContentfulFactory<ContentfulEventCategory, EventCategory> _eventCategoryFactory)
        {
            EventCategoryFactory = _eventCategoryFactory;
        }

        public List<EventCategory> ToModel(IEnumerable<ContentfulEventCategory> entries)
        {
            var eventCategoryList = new List<EventCategory>();
            foreach (var eventCategory in entries)
            {
                var eventCategoryItem = EventCategoryFactory.ToModel(eventCategory);
                eventCategoryList.Add(eventCategoryItem);
            }

            return eventCategoryList;
        }
    }
}