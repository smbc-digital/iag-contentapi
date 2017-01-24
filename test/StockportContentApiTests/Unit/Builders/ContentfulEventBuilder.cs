﻿using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulEventBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _teaser = "teaser";
        private Asset _image = new Asset
            {
                File = new Contentful.Core.Models.File()
                {
                    Url = "image-url.jpg",
                },
                SystemProperties = new SystemProperties { Type = "Asset" }
            };
        private string _description = "description";
        private string _fee = "fee";
        private string _location = "location";
        private string _submittedby = "submittedBy";
        private DateTime _eventDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private string _startTime = "10:00";
        private string _endTime = "17:00";
        private int _occurences = -1;
        private EventFrequency _eventFrequency = StockportContentApi.Model.EventFrequency.None;
        private List<Crumb> _breadcrumbs = new List<Crumb> {new Crumb("Events", "", "events")};
        private List<Asset> _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
        private List<string> _categories = new List<string> {"Category 1", "Category 2"};

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
                Categories = _categories
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
    }
}
