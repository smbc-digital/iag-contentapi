using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Razor.TagHelpers;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulEventBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _teaser = "teaser";
        private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private string _description = "description";
        private string _fee = "fee";
        private string _location = "location";
        private string _submittedby = "submittedBy";
        private DateTime _eventDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private string _startTime = "10:00";
        private string _endTime = "17:00";
        private int _occurences = -1;
        private EventFrequency _eventFrequency = EventFrequency.None;
        private List<Crumb> _breadcrumbs = new List<Crumb> { new Crumb("Events", "", "events") };
        private List<Asset> _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
        private List<string> _categories = new List<string> {"category 1", "category 2"};
        private MapPosition _mapPosition = new MapPosition() {Lat=53.5, Lon = -2.5};
        private string _bookingInformation = "booking information";
        private bool _featured = false;
        public ContentfulEventSys _sys = new ContentfulEventSys();
        private List<string> _tags = new List<string>{"tag 1", "tag 2"};

        public ContentfulEvent Build()
        {
            return new ContentfulEvent()
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Image = _image,
                Description = _description,
                Fee = _fee,
                Location = _location,
                SubmittedBy = _submittedby,
                EventDate = _eventDate,
                StartTime = _startTime,
                EndTime = _endTime,
                Occurences = _occurences,
                Frequency = _eventFrequency,
                Breadcrumbs = _breadcrumbs,
                Documents = _documents,
                Categories = _categories,
                MapPosition = _mapPosition,
                BookingInformation = _bookingInformation,
                Featured =  _featured,
                Sys = _sys,
                Tags = _tags
            };
        }

        public ContentfulEventBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulEventBuilder Occurrences(int occurrences)
        {
            _occurences = occurrences;
            return this;
        }

        public ContentfulEventBuilder Frequency(EventFrequency frequency)
        {
            _eventFrequency = frequency;
            return this;
        }

        public ContentfulEventBuilder EventDate(DateTime eventDate)
        {
            _eventDate = eventDate;
            return this;
        }

        public ContentfulEventBuilder EventCategory(List<string> categoriesList)
        {
            _categories = categoriesList;
            return this;
        }

        public ContentfulEventBuilder MapPosition(MapPosition mapPosition)
        {
            _mapPosition = mapPosition;
            return this;
        }

        public ContentfulEventBuilder Featured(bool featured)
        {
            _featured = featured;
            return this;
        }

        public ContentfulEventBuilder UpdatedAt(DateTime updatedAt)
        {
            _sys.UpdatedAt = updatedAt;
            return this;
        }

        public ContentfulEventBuilder Tags(List<string> tags )
        {
            _tags = tags;
            return this;
        }
    }
}
