using System;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using System.Collections.Generic;

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
            var thumbnailImage = ConvertToThumbnail(image);
            var description = (string)entry.fields.description ?? string.Empty;
            DateTime sunriseDate = DateComparer.DateFieldToDate(entry.fields.sunriseDate);
            DateTime sunsetDate = DateComparer.DateFieldToDate(entry.fields.sunsetDate);
            var fee = (string)entry.fields.fee ?? string.Empty;
            var location = (string)entry.fields.location ?? string.Empty;
            var submittedBy = (string)entry.fields.submittedBy ?? string.Empty;
            var longitude = (string)entry.fields.longitude ?? string.Empty;
            var latitude = (string)entry.fields.latitude ?? string.Empty;
            bool featured = entry.fields.featured ?? false;
            DateTime eventDate = DateComparer.DateFieldToDate(entry.fields.eventDate);
            var startTime = (string)entry.fields.startTime ?? string.Empty;
            var endTime = (string)entry.fields.endTime ?? string.Empty;
            var breadcrumbs = new List<Crumb>() { new Crumb("Events", string.Empty, "events") };

            return new Event(title, slug, teaser, image, thumbnailImage, description, sunriseDate, sunsetDate, fee, location,
                submittedBy, longitude, latitude, featured, eventDate, startTime, endTime, breadcrumbs);
           
        }

        private static dynamic ConvertToThumbnail(dynamic thumbnailImage)
        {
            thumbnailImage += !string.IsNullOrEmpty(thumbnailImage) ? "?h=250" : "";
            return thumbnailImage;
        }
    }
}
