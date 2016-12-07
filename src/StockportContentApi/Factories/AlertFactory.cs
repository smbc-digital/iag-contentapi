using System;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class AlertFactory : IFactory<Alert>
    {
        public Alert Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null) return new NullAlert();
            var title = (string)entry.fields.title ?? string.Empty;
            var subHeading = (string)entry.fields.subHeading ?? string.Empty;
            var body = (string)entry.fields.body ?? string.Empty;
            var severity = (string)entry.fields.severity ?? string.Empty;
            DateTime sunriseDate = DateComparer.DateFieldToDate(entry.fields.sunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(entry.fields.sunsetDate);

            return new Alert(title, subHeading, body, severity, sunriseDate, sunsetDate);
        }
    }
}