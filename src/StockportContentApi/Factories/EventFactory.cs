using System;
using System.Collections.Generic;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class EventFactory : IFactory<Event>
    {
        public Event Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var title = (string)entry.fields.title ?? string.Empty;
            var slug = (string)entry.fields.slug ?? string.Empty;
            var teaser = (string)entry.fields.teaser ?? string.Empty;
            var image = contentfulResponse.GetImageUrl(entry.fields.image);           
            var description = (string)entry.fields.description ?? string.Empty;
            DateTime sunriseDate = DateComparer.DateFieldToDate(entry.fields.sunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(entry.fields.sunsetDate);         

            return new Event(title, slug, teaser, image, description, sunriseDate, sunsetDate);
            
        }     
    }
}
