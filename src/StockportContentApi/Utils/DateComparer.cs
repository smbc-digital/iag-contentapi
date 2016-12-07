using System;
using System.Globalization;

namespace StockportContentApi.Utils
{
    public class DateComparer
    {
        private readonly ITimeProvider _timeProvider;

        public DateComparer(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public static DateTime DateFieldToDate(dynamic date)
        {
            return !IsValidDateTime(date) 
                ? DateTime.MinValue.ToUniversalTime()
                : ((DateTimeOffset)DateTimeOffset.Parse(date.ToString("u"), CultureInfo.InvariantCulture)).UtcDateTime;
        }

        private static bool IsValidDateTime(dynamic date)
        {
            DateTime datetime;
            return date != null && DateTime.TryParse(date.ToString(), out datetime);
        }

        public bool DateNowIsWithinSunriseAndSunsetDates(DateTime sunriseDate, DateTime sunsetDate)
        {
            return (sunriseDate.Equals(DateTime.MinValue) || _timeProvider.Now() >= sunriseDate)
                   && (sunsetDate.Equals(DateTime.MinValue) || _timeProvider.Now() <= sunsetDate);
        }

        public bool SunriseDateIsBetweenStartAndEndDates(DateTime sunriseDate, DateTime startDate, DateTime endDate)
        {
            return sunriseDate.Date >= startDate.Date && sunriseDate.Date <= endDate.Date && sunriseDate <= _timeProvider.Now();
        }
    }
}