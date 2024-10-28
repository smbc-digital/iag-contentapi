namespace StockportContentApiTests.Unit.Builders;

public class ContentfulArticleForSiteMapBuilder
{
    private string _slug = "slug";
    private DateTime _sunriseDate = new(2016, 1, 10, 0, 0, 0, DateTimeKind.Utc);
    private DateTime _sunsetDate = new(2017, 1, 20, 0, 0, 0, DateTimeKind.Utc);
    private List<ContentfulSectionForSiteMap> _sections = new() { new ContentfulSectionForSiteMapBuilder().Build() };
    private string _systemId = "id";
    private string _contentTypeSystemId = "id";
    private readonly DateTime _updatedAt = DateTime.Now;
    private readonly DateTime _createdAt = DateTime.Now;

    public ContentfulArticleForSiteMap Build()
        => new()
        {
            Slug = _slug,
            Sections = _sections,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Sys = new SystemProperties
            {
                ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                Id = _systemId,
                UpdatedAt = _updatedAt,
                CreatedAt = _createdAt,
            }
        };

    public ContentfulArticleForSiteMapBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }


    public ContentfulArticleForSiteMapBuilder WithSection(ContentfulSectionForSiteMap section)
    {
        _sections.Add(section);
        return this;
    }

    public ContentfulArticleForSiteMapBuilder WithOutSection()
    {
        _sections = new List<ContentfulSectionForSiteMap>();
        return this;
    }

    public ContentfulArticleForSiteMapBuilder WithSystemId(string id)
    {
        _systemId = id;
        return this;
    }

    public ContentfulArticleForSiteMapBuilder WithSystemContentTypeId(string id)
    {
        _contentTypeSystemId = id;
        return this;
    }

    public ContentfulArticleForSiteMapBuilder WithSunrise(DateTime sunrise)
    {
        _sunriseDate = sunrise;
        return this;
    }

    public ContentfulArticleForSiteMapBuilder WithSunset(DateTime sunset)
    {
        _sunsetDate = sunset;
        return this;
    }
}