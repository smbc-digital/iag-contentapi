namespace StockportContentApiTests.Unit.Builders;

public class ContentfulTopicForSiteMapBuilder
{
    private string _slug = "slug";
    private List<ContentfulSectionForSiteMap> _sections = new();

    public ContentfulTopicForSiteMap Build()
        => new()
        {
            Slug = _slug,
            SunriseDate = DateTime.MaxValue,
            SunsetDate = DateTime.MaxValue,
            Sections = _sections,
            Sys = new SystemProperties()
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } },
                Id = "id"
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