﻿using System;
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

        public bool DateNowIsWithinSunriseAndSunsetDates(DateTime sunriseDate, DateTime sunsetDate)
        {
            var sunriseCheck = sunriseDate.Equals(DateTime.MinValue) || _timeProvider.Now() >= sunriseDate;
            var sunsetCheck = sunsetDate.Equals(DateTime.MinValue) || _timeProvider.Now() <= sunsetDate;

            return sunriseCheck
                   && sunsetCheck;
        }

        public bool SunriseDateIsBetweenStartAndEndDates(DateTime sunriseDate, DateTime startDate, DateTime endDate)
        {
            return sunriseDate.Date >= startDate.Date && sunriseDate.Date <= endDate.Date && sunriseDate <= _timeProvider.Now();
        }

        private static bool IsValidDateTime(dynamic date)
        {
            DateTime datetime;
            return date != null && DateTime.TryParse(date.ToString(), out datetime);
        }
    }
}