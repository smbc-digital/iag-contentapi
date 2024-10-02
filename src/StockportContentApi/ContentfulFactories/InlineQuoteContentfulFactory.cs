namespace StockportContentApi.ContentfulFactories;

public class InlineQuoteContentfulFactory : IContentfulFactory<ContentfulInlineQuote, InlineQuote>
{
    public InlineQuote ToModel(ContentfulInlineQuote entry) =>
        new()
        {
            Image = entry.Image.File.Url,
            ImageAltText = entry.ImageAltText,
            Quote = entry.Quote,
            Author = entry.Author,
            Slug = entry.Slug,
            Theme = entry.Theme
        };
}
