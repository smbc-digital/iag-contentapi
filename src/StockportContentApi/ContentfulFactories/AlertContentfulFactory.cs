namespace StockportContentApi.ContentfulFactories;

public class AlertContentfulFactory : IContentfulFactory<ContentfulAlert, Alert>
{
    public Alert ToModel(ContentfulAlert entry)
    {
        return new Alert(entry.Title, entry.SubHeading, entry.Body, entry.Severity, entry.SunriseDate, entry.SunsetDate, entry.Slug, entry.IsStatic);
    }
}