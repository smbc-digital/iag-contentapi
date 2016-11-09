using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using StockportContentApi.Model;
using System.Linq;

namespace StockportContentApi.Factories
{
    public class RedirectsFactory : IFactory<RedirectDictionary>
    {
        public RedirectDictionary Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null) return new NullRedirectDictionary();

            var redirectDynamic = (IEnumerable<KeyValuePair<string, JToken>>)entry.fields.redirects;
            var newDict = redirectDynamic.ToDictionary(kvp => kvp.Key, kvp => kvp.Value.ToString());

            return new RedirectDictionary(newDict);
        }
    }
}