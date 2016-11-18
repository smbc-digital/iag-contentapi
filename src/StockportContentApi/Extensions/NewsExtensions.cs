using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StockportContentApi.Extensions
{
    public static class NewsExtensions
    {
        public static IEnumerable<News> GetTheCategories(this IEnumerable<News> news, out List<string> categories)
        {
            var categoriesList = new List<string>();

            news.ToList().ForEach(n => categoriesList.AddRange(n.Categories));

            categories = categoriesList;
            return news;
        }

        public static IEnumerable<News> GetNewsDates(this IEnumerable<News> news, out List<DateTime> dates)
        {
            var datesList = new List<DateTime>();

            foreach (var item in news.ToList())
            {
                if (!datesList.Any(d => d.Month.Equals(item.SunriseDate.Month) && d.Year.Equals(item.SunriseDate.Year)))
                {
                    datesList.Add(new DateTime(item.SunriseDate.Year, item.SunriseDate.Month, 01));
                }
            }

            dates = datesList;
            return news;
        }
    }
}
