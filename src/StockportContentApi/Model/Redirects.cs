using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Redirects
    {
        public readonly Dictionary<string, RedirectDictionary> ShortUrlRedirects;
        public readonly Dictionary<string, RedirectDictionary> LegacyUrlRedirects;

        public Redirects(Dictionary<string, RedirectDictionary> shortUrlRedirects, Dictionary<string, RedirectDictionary> legacyUrlRedirects)
        {
            ShortUrlRedirects = shortUrlRedirects;
            LegacyUrlRedirects = legacyUrlRedirects;
        }
    }
}