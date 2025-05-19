namespace StockportContentApi.Utils;

public class DateComparer
{
    private readonly ITimeProvider _timeProvider;

    public DateComparer(ITimeProvider timeProvider) =>
        _timeProvider = timeProvider;

    public static DateTime DateFieldToDate(dynamic date) =>
        !IsValidDateTime(date)
            ? DateTime.MinValue.ToUniversalTime()
            : ((DateTimeOffset)DateTimeOffset.Parse(date.ToString("u"), CultureInfo.InvariantCulture)).UtcDateTime;

    public bool DateNowIsWithinSunriseAndSunsetDates(
        DateTime sunriseDate,
        DateTime? sunsetDate = null)
    {
        sunsetDate ??= DateTime.MaxValue;
        bool sunriseCheck = sunriseDate.Equals(DateTime.MinValue) || _timeProvider.Now() >= sunriseDate;
        bool sunsetCheck = sunsetDate.Equals(DateTime.MinValue) || _timeProvider.Now() <= sunsetDate;

        return sunriseCheck && sunsetCheck;
    }

    public bool DateNowIsAfterSunriseDate(DateTime sunriseDate) =>
        sunriseDate < _timeProvider.Now();
    
    public bool DateNowIsNotBetweenHiddenRange(DateTime? hiddenFrom, DateTime? hiddenTo)
    {
        DateTime now = DateTime.Now;

        return hiddenFrom > now
            || (hiddenTo < now && !hiddenTo.Equals(DateTime.MinValue))
            || (hiddenFrom.Equals(DateTime.MinValue) && hiddenTo.Equals(DateTime.MinValue))
            || (hiddenFrom is null && hiddenTo is null);
    }

    public bool SunriseDateIsBetweenStartAndEndDates(DateTime sunriseDate, DateTime startDate, DateTime endDate) =>
        sunriseDate.Date >= startDate.Date
            && sunriseDate.Date <= endDate.Date
            && sunriseDate <= _timeProvider.Now();

    public bool SunsetDateIsInThePast(DateTime sunsetDate) =>
        sunsetDate.Date < _timeProvider.Now();

    public bool EventDateIsBetweenStartAndEndDates(DateTime eventDate, DateTime startDate, DateTime endDate) =>
        eventDate.Date >= startDate.Date
            && eventDate.Date <= endDate.Date;

    public bool EventDateIsBetweenTodayAndLater(DateTime eventDate) =>
        eventDate.Date >= _timeProvider.Now().Date;

    private static bool IsValidDateTime(dynamic date) =>
        date is not null
            && DateTime.TryParse(date.ToString(), out DateTime datetime);
}