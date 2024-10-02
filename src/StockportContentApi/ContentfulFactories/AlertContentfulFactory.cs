namespace StockportContentApi.ContentfulFactories;

public class AlertContentfulFactory : IContentfulFactory<ContentfulAlert, Alert>
{
    public Alert ToModel(ContentfulAlert entry) =>
        new(entry.Title, entry.SubHeading, entry.Body, entry.Severity, entry.SunriseDate, entry.SunsetDate, entry.Slug, entry.IsStatic, entry.Image.File.Url);
}