using System;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class EventBannerFactory: IFactory<EventBanner>
    {
        public EventBanner Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null) return new NullEventBanner();
            var title = (string)entry.fields.title ?? string.Empty;
            var teaser = (string)entry.fields.teaser ?? string.Empty;
            var icon = (string)entry.fields.icon ?? string.Empty;
            var link = (string)entry.fields.link ?? string.Empty;

            return new EventBanner(title, teaser, icon, link);
        }
    }
}
