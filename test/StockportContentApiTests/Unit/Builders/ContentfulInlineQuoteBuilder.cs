namespace StockportContentApiTests.Unit.Builders;

public class ContentfulInlineQuoteBuilder
{
    private string _imageAltText = "image alt text";
    private string _quote = "quote";
    private string _author = "author";
    private string _slug = "slug";
    private EColourScheme _theme = EColourScheme.Blue;

    public ContentfulInlineQuote Build()
        => new()
        {
            Image = new ContentfulAssetBuilder().Url("image-url.jpg").Build(),
            ImageAltText = _imageAltText,
            Quote = _quote,
            Author = _author,
            Slug = _slug,
            Theme = _theme,
        };

    public ContentfulInlineQuoteBuilder WithImageAltText(string imageAltText)
    {
        _imageAltText = imageAltText;
        return this;
    }

    public ContentfulInlineQuoteBuilder WithQuote(string quote)
    {
        _quote = quote;
        return this;
    }
    
    public ContentfulInlineQuoteBuilder WithAuthor(string author)
    {
        _author = author;
        return this;
    }

    public ContentfulInlineQuoteBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulInlineQuoteBuilder WithTheme(EColourScheme theme)
    {
        _theme = theme;
        return this;
    }
}