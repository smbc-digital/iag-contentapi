namespace StockportContentApiTests.Unit.Builders;

public class ContentfulSectionBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _metaDescription = "metaDescription";
    private string _body = "body";
    private DateTime _sunriseDate = DateTime.MinValue;
    private DateTime _sunsetDate = DateTime.MinValue;
    private DateTime _updatedAt = DateTime.Now;
    private List<Asset> _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
    private List<ContentfulProfile> _profiles = new List<ContentfulProfile> {
        new ContentfulProfileBuilder().Build() };
    private List<ContentfulAlert> _alertsInline = new List<ContentfulAlert>
    {
        new ContentfulAlertBuilder().Build()
    };

    public ContentfulSection Build()
    {
        return new ContentfulSection
        {
            Title = _title,
            Slug = _slug,
            MetaDescription = _metaDescription,
            Body = _body,
            Profiles = _profiles,
            Documents = _documents,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            UpdatedAt = _updatedAt,
            AlertsInline = _alertsInline,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
                UpdatedAt = _updatedAt
            }
        };
    }
}
