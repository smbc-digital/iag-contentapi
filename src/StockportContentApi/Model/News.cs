using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class News
    {
        public string Title { get; }
        public string Slug { get; }
        public string Teaser { get; }
        public string Image { get; }
        public string ThumbnailImage { get; }
        public string Body { get; set; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }
        public List<Crumb> Breadcrumbs { get; set; }
        public List<string> Tags { get; set; }
        public List<Alert> Alerts { get; }
        public List<Document> Documents { get; }
        public List<string> Categories { get; }

        public News(string title, string slug, string teaser, string image, string thumbnailImage, string body, DateTime sunriseDate, DateTime sunsetDate, List<Crumb> breadcrumbs, List<Alert> alerts, List<string> tags, List<Document> documents, List<string> categories)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Image = image;
            ThumbnailImage = thumbnailImage;
            Body = body;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            Tags = tags;
            Documents = documents;
            Categories = categories;
        }
    }

    public class NullNews : News
    {
        public NullNews()
            : base(
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new DateTime(),

                new DateTime(), new List<Crumb>(), new List<Alert>(), new List<string>(), new List<Document>(), new List<string>())
        { }
    }
}