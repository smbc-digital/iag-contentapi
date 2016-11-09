using System;
using System.Collections.Generic;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Factories
{
    public class NewsFactory : IFactory<News>
    {
        private readonly IBuildContentTypesFromReferences<Alert> _alertListFactory;
        private readonly IBuildContentTypesFromReferences<Document> _documentListFactory;

        public NewsFactory(IBuildContentTypesFromReferences<Alert> alertListFactory, IBuildContentTypesFromReferences<Document> documentListFactory)
        {
            _alertListFactory = alertListFactory;
            _documentListFactory = documentListFactory;
        }

        public News Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var title = (string)entry.fields.title ?? string.Empty;
            var slug = (string)entry.fields.slug ?? string.Empty;
            var teaser = (string)entry.fields.teaser ?? string.Empty;

            var image = contentfulResponse.GetImageUrl(entry.fields.image);
            var thumbnailImage = contentfulResponse.GetImageUrl(entry.fields.image);
            thumbnailImage = ConvertToThumbnail(thumbnailImage); 

            var body = (string)entry.fields.body ?? string.Empty;

            DateTime sunriseDate = SunriseSunsetDates.DateFieldToDate(entry.fields.sunriseDate);
            DateTime sunsetDate = SunriseSunsetDates.DateFieldToDate(entry.fields.sunsetDate);

            var breadcrumbs = new List<Crumb>() { new Crumb("News", string.Empty, "news") };
            var alerts = _alertListFactory.BuildFromReferences(entry.fields.alerts, contentfulResponse);
            var documents = _documentListFactory.BuildFromReferences(entry.fields.documents, contentfulResponse);

            var tags = GetListOfStrings(entry.fields.tags);
            var categories = GetListOfStrings(entry.fields.categories);

            return new News(title, slug, teaser, image, thumbnailImage, body, sunriseDate, sunsetDate, breadcrumbs,alerts, tags, documents, categories);
        }

        private static dynamic ConvertToThumbnail(dynamic thumbnailImage)
        {
            thumbnailImage += !string.IsNullOrEmpty(thumbnailImage) ? "?h=250" : "";
            return thumbnailImage;
        }


        private static List<string> GetListOfStrings(dynamic entries)
        {
            var listOfStrings = new List<string>();
            if (entries == null) return listOfStrings;

            foreach (var entry in entries)
            {
                listOfStrings.Add((string)entry);
            }
            return listOfStrings;
        }
    }
}