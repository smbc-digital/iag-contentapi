using System;


namespace StockportContentApi.Utils
{
    public class SunriseSunsetDates
    {

        private readonly ITimeProvider _timeProvider;

        public SunriseSunsetDates(ITimeProvider timeProvider )
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

        public static DateTime MakeMinValueDateIfNull(dynamic date)
        {
            return date ?? DateTime.MinValue;
        }

       public bool CheckIsWithinSunriseAndSunsetDates(DateTime sunriseDate, DateTime sunsetDate)
        {           
            return IsNowBetweenSunriseAndSunsetDates(sunriseDate, sunsetDate);
        }

        private bool IsNowBetweenSunriseAndSunsetDates(DateTime sunriseDate, DateTime sunsetDate)
        {
            return (sunriseDate.Equals(DateTime.MinValue) || _timeProvider.Now() >= sunriseDate) 
                   && (sunsetDate.Equals(DateTime.MinValue) || _timeProvider.Now() <= sunsetDate);
        }
    }
}
