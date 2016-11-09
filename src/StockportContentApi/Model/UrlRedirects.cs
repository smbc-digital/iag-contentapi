using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class RedirectDictionary : Dictionary<string, string>
    {
        public RedirectDictionary() { }
        public RedirectDictionary(IDictionary<string, string> dict) : base(dict) { }
    }

    public class NullRedirectDictionary : RedirectDictionary
    { }
}