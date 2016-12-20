using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using StockportContentApi.Utils;

namespace StockportContentApi.Extensions
{
    public static class EventsExtensions
    {      
        public static IEnumerable<Event> GetEventDates(this IEnumerable<Event> events, out List<DateTime> dates, ITimeProvider timeProvider)
        {
            var datesList = new List<DateTime>();

            foreach (var item in events.ToList())
            {
                var isSunriseDateIsInThePast = item.SunriseDate <= timeProvider.Now();

                var isDateAlreadyInList = datesList.Any(d => d.Month.Equals(item.SunriseDate.Month) && d.Year.Equals(item.SunriseDate.Year));

                if (!isDateAlreadyInList && isSunriseDateIsInThePast)
                {
                    datesList.Add(new DateTime(item.SunriseDate.Year, item.SunriseDate.Month, 01, 0, 0, 0, DateTimeKind.Utc));
                }
            }

            dates = datesList;
            return events;
        }
    }
}
