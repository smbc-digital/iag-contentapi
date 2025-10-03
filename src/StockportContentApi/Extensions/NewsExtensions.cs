namespace StockportContentApi.Extensions;

public static class NewsExtensions
{
    public static IEnumerable<News> GetNewsDates(this IEnumerable<News> news, out List<DateTime> dates, ITimeProvider timeProvider)
    {
        List<DateTime> datesList = new();

        foreach (News item in news.ToList())
        {
            bool isSunriseDateInThePast = item.SunriseDate <= timeProvider.Now();
            bool isDateAlreadyInList = datesList.Any(date => date.Month.Equals(item.SunriseDate.Month) && date.Year.Equals(item.SunriseDate.Year));

            if (!isDateAlreadyInList && isSunriseDateInThePast)
                datesList.Add(new DateTime(item.SunriseDate.Year, item.SunriseDate.Month, 01, 0, 0, 0, DateTimeKind.Utc));
        }

        dates = datesList;

        return news;
    }

    public static IEnumerable<News> GetNewsYears(this IEnumerable<News> news, out List<int> years, ITimeProvider timeProvider)
    {
        List<int> yearList = new();

        foreach (News item in news.ToList())
        {
            bool isSunriseDateInThePast = item.SunriseDate <= timeProvider.Now();
            bool isYearAlreadyInList = yearList.Contains(item.SunriseDate.Year);

            if (!isYearAlreadyInList && isSunriseDateInThePast)
                yearList.Add(item.SunriseDate.Year);
        }

        years = yearList.OrderByDescending(y => y).ToList();

        return news;
    }
}