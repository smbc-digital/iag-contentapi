namespace StockportContentApi.Extensions;

public static class NewsExtensions
{
    public static IEnumerable<News> GetNewsDates(this IEnumerable<News> news, out List<DateTime> dates, ITimeProvider timeProvider)
    {
        List<DateTime> datesList = new();

        foreach (News item in news.ToList())
        {
            bool isSunriseDateIsInThePast = item.SunriseDate <= timeProvider.Now();

            bool isDateAlreadyInList = datesList.Any(date => date.Month.Equals(item.SunriseDate.Month) && date.Year.Equals(item.SunriseDate.Year));

            if (!isDateAlreadyInList && isSunriseDateIsInThePast)
            {
                datesList.Add(new DateTime(item.SunriseDate.Year, item.SunriseDate.Month, 01, 0, 0, 0, DateTimeKind.Utc));
            }
        }

        dates = datesList;
        return news;
    }
}
