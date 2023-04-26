namespace StockportContentApi.Extensions;

public static class NewsExtensions
{
    public static IEnumerable<News> GetNewsDates(this IEnumerable<News> news, out List<DateTime> dates, ITimeProvider timeProvider)
    {
        var datesList = new List<DateTime>();

        foreach (var item in news.ToList())
        {
            var isSunriseDateIsInThePast = item.SunriseDate <= timeProvider.Now();

            var isDateAlreadyInList = datesList.Any(d => d.Month.Equals(item.SunriseDate.Month) && d.Year.Equals(item.SunriseDate.Year));

            if (!isDateAlreadyInList && isSunriseDateIsInThePast)
            {
                datesList.Add(new DateTime(item.SunriseDate.Year, item.SunriseDate.Month, 01, 0, 0, 0, DateTimeKind.Utc));
            }
        }

        dates = datesList;
        return news;
    }
}
