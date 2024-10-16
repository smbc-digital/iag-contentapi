﻿namespace StockportContentApiTests.Unit.Builders;

public class ContentfulRedirectBuilder
{
    private readonly string _title = "_title";
    private readonly Dictionary<string, string> _redirects = new() { { "a-url", "another-url" } };
    private readonly Dictionary<string, string> _legacyUrls = new() { { "some-url", "another-url" } };

    public ContentfulRedirect Build()
        => new()
        {
            Title = _title,
            LegacyUrls = _legacyUrls,
            Redirects = _redirects
        };

    public ContentfulRedirect BuildForRouteTest()
        => new()
        {
            LegacyUrls = new Dictionary<string, string> { { "a-url", "another-url" }, { "start-url", "end-url" } },
            Redirects = new Dictionary<string, string>
            {
                { "starturl.fake/this-is-another-article", "redirecturl.fake/another-article" },
                { "starturl.fake/this-is-an-article/ghjgjk/gjyuy", "an article" },
                { "starturl.fake/counciltax", "an-article" },
                { "starturl.fake/bins", "redirecturl.fake/bins" },
                { "starturl.fake/healthystockport", "redirecturl.fake" }
            }
        };
}