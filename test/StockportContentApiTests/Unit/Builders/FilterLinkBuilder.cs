namespace StockportContentApiTests.Unit.Builders;

public class ContentfulFilterBuilder
{
    private string _slug = "slug";
    private string _title = "title";
    private string _displayName = "display name";
    private string _theme = "theme";
    private bool _highlight = false;

    public ContentfulFilter Build()
        => new()
        {
            Slug = _slug,
            Title = _title,
            DisplayName = _displayName,
            Theme = _theme,
            Highlight = _highlight,
        };
    
    public ContentfulFilterBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulFilterBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulFilterBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public ContentfulFilterBuilder WithTheme(string theme)
    {
        _theme = theme;
        return this;
    }

    public ContentfulFilterBuilder WithHighlight(bool highlight)
    {
        _highlight = highlight;
        return this;
    }
}