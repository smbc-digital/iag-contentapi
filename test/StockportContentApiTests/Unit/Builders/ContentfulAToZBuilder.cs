namespace StockportContentApiTests.Unit.Builders;

public class ContentfulAToZBuilder
{
    private string _title = "Vintage Village turns 6 years old";
    private readonly string _name = "Vintage Village turns six years old";
    private string _slug = "vintage-village-turns-6-years-old";
    private string _teaser = "The vintage village turned 6 with a great reception";
    private readonly string _displayOnAZ = "True";
    private List<string> _alternativeTitles = new() { "test1, test2, test3" };
    private readonly DateTime _sunriseDate = new(0001, 01, 01, 00, 00, 00);
    private readonly DateTime _sunsetDate = new(0001, 01, 01, 00, 00, 00);
    private SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "article" } }
    };

    public ContentfulAtoZ Build()
        => new()
        {
            Title = _title,
            Name = _name,
            Slug = _slug,
            Teaser = _teaser,
            DisplayOnAZ = _displayOnAZ,
            AlternativeTitles = _alternativeTitles,
            SunriseDate = _sunriseDate,
            SunsetDate = _sunsetDate,
            Sys = _sys
        };

    public ContentfulAToZBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulAToZBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulAToZBuilder Teaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }

    public ContentfulAToZBuilder AlternativeTitles(List<string> alternativeTitles)
    {
        _alternativeTitles = alternativeTitles;
        return this;
    }

    public ContentfulAToZBuilder Sys(string sysId)
    {
        _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = sysId } }
        };
        return this;
    }
}