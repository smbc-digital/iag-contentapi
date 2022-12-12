namespace StockportContentApi.Model
{
    public class Redirects
    {
        public Dictionary<string, RedirectDictionary> ShortUrlRedirects;
        public Dictionary<string, RedirectDictionary> LegacyUrlRedirects;

        public Redirects(Dictionary<string, RedirectDictionary> shortUrlRedirects, Dictionary<string, RedirectDictionary> legacyUrlRedirects)
        {
            ShortUrlRedirects = shortUrlRedirects;
            LegacyUrlRedirects = legacyUrlRedirects;
        }
    }

    public class ShortUrlRedirects
    {
        public Dictionary<string, RedirectDictionary> Redirects;

        public ShortUrlRedirects(Dictionary<string, RedirectDictionary> redirects)
        {
            Redirects = redirects;
        }
    }

    public class LegacyUrlRedirects
    {
        public Dictionary<string, RedirectDictionary> Redirects;

        public LegacyUrlRedirects(Dictionary<string, RedirectDictionary> redirects)
        {
            Redirects = redirects;
        }
    }
}