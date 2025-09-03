namespace StockportContentApiTests.Unit.Builders;

public class ContentfulAToZBuilder
{
    private string _title = "Vintage Village turns 6 years old";
    private List<string> _alternativeTitles = new() { "test1, test2, test3" };
    private readonly SystemProperties _sys = new()
    {
        ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "article" } }
    };

    public ContentfulAtoZ Build()
        => new()
        {
            Title = _title,
            Name = "Vintage Village turns six years old",
            Slug = "vintage-village-turns-6-years-old",
            Teaser = "The vintage village turned 6 with a great reception",
            DisplayOnAZ = "True",
            AlternativeTitles = _alternativeTitles,
            SunriseDate = DateTime.MinValue,
            SunsetDate = DateTime.MinValue,
            Sys = _sys
        };

    public ContentfulAToZBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulAToZBuilder AlternativeTitles(List<string> alternativeTitles)
    {
        _alternativeTitles = alternativeTitles;
        return this;
    }
}