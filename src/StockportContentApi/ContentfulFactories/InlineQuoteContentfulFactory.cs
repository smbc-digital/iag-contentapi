using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class InlineQuoteContentfulFactory : IContentfulFactory<ContentfulInlineQuote, InlineQuote>
    {
        public InlineQuote ToModel(ContentfulInlineQuote entry)
        {
            return new InlineQuote()
            {
                Image = entry.Image.File.Url,
                ImageAltText = entry.ImageAltText,
                Quote = entry.Quote,
                Author = entry.Author,
                Slug = entry.Slug
            };
        }
    }
}
