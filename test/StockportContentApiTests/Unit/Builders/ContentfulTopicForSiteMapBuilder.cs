namespace StockportContentApiTests.Unit.Builders;

public class ContentfulTopicForSiteMapBuilder
{
    private string _slug = "slug";
    private readonly DateTime _sunriseDate = DateTime.MinValue;
    private readonly DateTime _sunsetDate = DateTime.MaxValue;
    private List<ContentfulSectionForSiteMap> _sections = new();
    private readonly string _systemId = "id";
    private readonly string _contentTypeSystemId = "id";

    public ContentfulTopicForSiteMap Build()
        => new()
        {
            Slug = _slug,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Sections = _sections,
            Sys = new SystemProperties()
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                Id = _systemId
            }
        };

    public ContentfulTopicForSiteMapBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulTopicForSiteMapBuilder WithSections(List<ContentfulSectionForSiteMap> sections)
    {
        _sections = sections;
        return this;
    }
}