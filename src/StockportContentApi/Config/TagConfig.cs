namespace StockportContentApi.Config
{
    public class TagConfig
    {
        public readonly string ClosingTag;

        public readonly string OpeningTag;

        public TagConfig(string openingTag, string closingTag)
        {
            OpeningTag = openingTag;
            ClosingTag = closingTag;
        }
    }
}