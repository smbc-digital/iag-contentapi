using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class AlertContentfulFactory : IContentfulFactory<Entry<ContentfulAlert>, Alert>
    {
        public Alert ToModel(Entry<ContentfulAlert> entry)
        {
            return new Alert(entry.Fields.Title, entry.Fields.SubHeading, entry.Fields.Body, entry.Fields.Severity, entry.Fields.SunriseDate, entry.Fields.SunsetDate);
        }
    }
}