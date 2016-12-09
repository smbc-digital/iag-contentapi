using System;

namespace StockportContentApi.Model
{
    public class Event
    {
        public string Title { get; }
        public string Slug { get; }
        public string Teaser { get; }
        public string Image { get; }     
        public string ThumbnailImage { get; }     
        public string Description { get; set; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }

        public Event(string title, string slug, string teaser, string image, string thumbnailImage, string description, DateTime sunriseDate,
            DateTime sunsetDate)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Image = image;
            Description = description;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            ThumbnailImage = thumbnailImage;
        }
    }

    public class NullEvent : Event
    {
        public NullEvent()
            : base(
                string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new DateTime(), new DateTime())
        { }
    }
}
