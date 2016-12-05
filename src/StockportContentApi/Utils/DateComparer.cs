using System;

namespace StockportContentApi.Utils
{
    public class DateComparer
    {
        private readonly ITimeProvider _timeProvider;

        public DateComparer(ITimeProvider timeProvider)
        {
            _timeProvider = timeProvider;
        }

        public static DateTime DateFieldToDate(dynamic fieldDate)
        {
            DateTime sunriseDate = MakeMinValueDateIfNull(fieldDate);
            DateTime outSunriseDate;
            DateTime.TryParse(sunriseDate.ToString("yyyy-MM-ddTHH:mm:sszzz") ?? string.Empty, out outSunriseDate);
            return outSunriseDate;
        }

        private static DateTime MakeMinValueDateIfNull(dynamic date)
        {
            return date ?? DateTime.MinValue;
        }

        public bool DateNowIsWithinSunriseAndSunsetDates(DateTime sunriseDate, DateTime sunsetDate)
        {
            return (sunriseDate.Equals(DateTime.MinValue) || _timeProvider.Now() >= sunriseDate)
                   && (sunsetDate.Equals(DateTime.MinValue) || _timeProvider.Now() <= sunsetDate);
        }

        public bool SunriseDateIsBetweenStartAndEndDates(DateTime sunriseDate, DateTime startDate, DateTime endDate)
        {
            return sunriseDate >= startDate && sunriseDate < endDate && sunriseDate <= _timeProvider.Now();
        }
    }
}