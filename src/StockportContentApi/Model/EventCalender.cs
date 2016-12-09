using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class EventCalender
    {   
        public List<Event> Events { get; private set; }      

        public void SetEvents(List<Event> events)
        {
            Events = events;
        }
    } 
}
