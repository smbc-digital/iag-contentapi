using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class EventCalender
    {   
        public List<Event> Events { get; private set; }
        public List<string> Categories { get; private set; }

        public void SetEvents(List<Event> events, List<string> categories )
        {
            Events = events;
            Categories = categories;
        }
    } 
}
