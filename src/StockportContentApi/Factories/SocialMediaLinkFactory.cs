using System;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class SocialMediaLinkFactory : IFactory<SocialMediaLink>
    {
        public SocialMediaLink Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null || entry.sys == null) return null;

            var fields = entry.fields;

            var title = (string)fields.title ?? (string)fields.name ?? string.Empty;
            var slug = (string)fields.slug ?? string.Empty;
            var icon = (string)fields.icon ?? string.Empty;
            var url = (string)fields.url ?? string.Empty;

            return new SocialMediaLink(title, slug, url, icon);
        }
    }
}
