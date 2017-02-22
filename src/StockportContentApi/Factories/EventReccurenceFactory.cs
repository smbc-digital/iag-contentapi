using System;
using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class EventReccurenceFactory
    {
        private readonly Dictionary<EventFrequency, Func<Event, int, Event>> _reccurenceDictionary =
            new Dictionary<EventFrequency, Func<Event, int, Event>>
            {
                { EventFrequency.None, (e, dayNum) => null },
                { EventFrequency.Daily, (e, dayNum) => GetReccuringEvent(e, e.EventDate.Date.AddDays(dayNum * 1)) },
                { EventFrequency.Weekly, (e, dayNum) => GetReccuringEvent(e, e.EventDate.Date.AddDays(dayNum * 7)) },
                { EventFrequency.Fortnightly, (e, dayNum) => GetReccuringEvent(e, e.EventDate.Date.AddDays(dayNum * 14)) },
                { EventFrequency.Monthly, (e, monthNum) => GetReccuringEvent(e, e.EventDate.Date.AddMonths(monthNum * 1)) },
                { EventFrequency.MonthlyDate, (e, monthNum) => GetReccuringEvent(e, e.EventDate.Date.AddMonths(monthNum * 1)) },
                { EventFrequency.MonthlyDay, (e, monthNum) => GetReccuringEvent(e, GetCorrespondingMonthsDay(e.EventDate, monthNum)) },
                { EventFrequency.Yearly, (e, monthNum) => GetReccuringEvent(e, e.EventDate.Date.AddMonths(monthNum * 12)) }
            };

        public List<Event> GetReccuringEventsOfEvent(Event eventItem)
        {
            var reoccuredEventsByFrequency = new List<Event>();
            for (var i = 1; i < eventItem.Occurences; i++)
            {
                var recurringEvent = _reccurenceDictionary[eventItem.Frequency].Invoke(eventItem, i);
                if (recurringEvent != null) reoccuredEventsByFrequency.Add(recurringEvent);
            }
            return reoccuredEventsByFrequency;
        }

        public static Event GetReccuringEvent(Event entry, DateTime newDate)
        {
            return new Event(entry.Title, entry.Slug, entry.Teaser, entry.ImageUrl, entry.Description, entry.Fee,
                             entry.Location, entry.SubmittedBy, newDate, entry.StartTime, entry.EndTime, entry.Occurences, 
                             entry.Frequency, entry.Breadcrumbs, entry.ThumbnailImageUrl, entry.Documents, entry.Categories, 
                             entry.MapPosition, entry.Featured, entry.BookingInformation, entry.UpdatedAt, entry.Tags);
        }

        private static DateTime GetCorrespondingMonthsDay(DateTime date, int occurrence)
        {
            var followingMonth = date.AddMonths(occurrence);
            var newDate = GetFirstMatchingDayOfMonth(date, followingMonth);

            return GetWeeklyDayInMonth(date, newDate);
        }

        private static DateTime GetFirstMatchingDayOfMonth(DateTime date, DateTime nextMonth)
        {
            var newDate = new DateTime(nextMonth.Year, nextMonth.Month, 1, date.Hour, date.Minute, date.Second, date.Kind);
            while (newDate.DayOfWeek != date.DayOfWeek)
                newDate = newDate.AddDays(1);
            return newDate;
        }

        private static DateTime GetWeeklyDayInMonth(DateTime date, DateTime newDate)
        {
            var dayOccurrence = (date.Day - 1) / 7 + 1;

            for (var i = 1; i < dayOccurrence; i++)
            {
                newDate = newDate.AddDays(7);
                if (newDate.AddDays(7).Month != newDate.Month) break;
            }
            return newDate;
        }
    }
}
