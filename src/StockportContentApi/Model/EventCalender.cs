using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class EventCalender
    {   
        public List<Event> Events { get; private set; }      
        public List<DateTime> Dates { get; private set; }

        public void SetEvents(List<Event> events)
        {
            Events = events;
        }

        public void SetDates(List<DateTime> dates)
        {
            Dates = dates;
        }
    } 
}
