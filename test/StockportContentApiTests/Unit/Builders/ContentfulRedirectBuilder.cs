using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulRedirectBuilder
    {
        private string _title = "_title";
        private Dictionary<string, string> _redirects = new Dictionary<string, string> {{"a-url", "another-url"}};
        private Dictionary<string, string> _legacyUrls = new Dictionary<string, string> { { "some-url", "another-url" } };
        private SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

        public ContentfulRedirect Build()
        {
            return new ContentfulRedirect
            {
                Title = _title,
                LegacyUrls = _legacyUrls,
                Redirects = _redirects
            };
        }

        public ContentfulRedirect BuildForRouteTest()
        {
            return new ContentfulRedirect
            {
                LegacyUrls = new Dictionary<string, string> { { "a-url", "another-url" }, { "start-url", "end-url" } },
                Redirects = new Dictionary<string, string> { {"starturl.fake/this-is-another-article", "redirecturl.fake/another-article" }, { "starturl.fake/this-is-an-article/ghjgjk/gjyuy", "an article" }, { "starturl.fake/counciltax", "an-article" }, { "starturl.fake/bins", "redirecturl.fake/bins" }, { "starturl.fake/healthystockport", "redirecturl.fake" } }
            };
        }
    }
}
