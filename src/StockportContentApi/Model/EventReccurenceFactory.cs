using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class EventReccurenceFactory
    {
        private readonly Dictionary<EventFrequency, Func<Event, int, Event>> _recurranceDictionary =
            new Dictionary<EventFrequency, Func<Event, int, Event>>
            {
                { EventFrequency.None, (e, dayNum) => null },
                { EventFrequency.Daily, (e, dayNum) => GetReccuringEvent(e, e.EventDate.Date.AddDays(dayNum * 1)) },
                { EventFrequency.Weekly, (e, dayNum) => GetReccuringEvent(e, e.EventDate.Date.AddDays(dayNum * 7)) },
                { EventFrequency.Fortnightly, (e, dayNum) => GetReccuringEvent(e, e.EventDate.Date.AddDays(dayNum * 14)) },
                { EventFrequency.Monthly, (e, monthNum) => GetReccuringEvent(e, e.EventDate.Date.AddMonths(monthNum * 1)) },
                { EventFrequency.Yearly, (e, monthNum) => GetReccuringEvent(e, e.EventDate.Date.AddMonths(monthNum * 12)) }
            };

        public List<Event> GetReccuringEventsOfEvent(Event eventItem)
        {
            var reoccuredEventsByFrequency = new List<Event>();
            for (var i = 1; i < eventItem.Occurences; i++)
            {
                var recurringEvent = _recurranceDictionary[eventItem.Frequency].Invoke(eventItem, i);
                if (recurringEvent != null) reoccuredEventsByFrequency.Add(recurringEvent);
            }
            return reoccuredEventsByFrequency;
        }

        public static Event GetReccuringEvent(Event entry, DateTime newDate)
        {
            return new Event(entry.Title, entry.Slug, entry.Teaser, entry.ImageUrl, entry.Description, entry.Fee,
                           entry.Location, entry.SubmittedBy, entry.Longitude, entry.Latitude, entry.Featured, newDate, entry.StartTime,
                           entry.EndTime, entry.Occurences, entry.Frequency, entry.Breadcrumbs);
        }
    }
}
