namespace StockportContentApi.Model
{
    public class BusinessIdToRedirects
    {
        public readonly RedirectDictionary ShortUrlRedirects;
        public readonly RedirectDictionary LegacyUrlRedirects;

        public BusinessIdToRedirects(IDictionary<string, string> shortUrlRedirects, IDictionary<string, string> legacyUrlRedirects)
        {
            ShortUrlRedirects = new RedirectDictionary(shortUrlRedirects);
            LegacyUrlRedirects = new RedirectDictionary(legacyUrlRedirects);
        }
    }

    public class NullBusinessIdToRedirects : BusinessIdToRedirects
    {
        public NullBusinessIdToRedirects() : base(new RedirectDictionary(), new RedirectDictionary()) { }
    }
}