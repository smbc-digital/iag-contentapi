using System;
using System.Collections.Generic;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class SubItemFactory : IFactory<SubItem>
    {
        public SubItem Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            if (entry == null || entry.fields == null || entry.sys == null) return null;

            var fields = entry.fields;
            var sys = entry.sys;

            var contentType = (string)sys.contentType.sys.id == "startPage" ? "start-page" : (string)sys.contentType.sys.id;
            var slug = (string)fields.slug ?? string.Empty;
            var title = (string)fields.title ?? (string)fields.name ?? string.Empty;
            var teaser = (string)fields.teaser ?? string.Empty;
            var icon = (string)fields.icon ?? string.Empty;
            DateTime sunriseDate = DateComparer.DateFieldToDate(fields.sunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(fields.sunsetDate);
            var image = contentfulResponse.GetImageUrl(fields.image);

            return new SubItem(slug, title, teaser, icon, contentType, sunriseDate,sunsetDate, image, new List<SubItem>());
        }

    }
}
