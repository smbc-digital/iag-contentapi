﻿using Contentful.Core.Models;
using StockportContentApi.Model;
using System;

namespace StockportContentApiTests.Unit.Builders
{
    class AlertBuilder
    {
        private string _title = "title";
        private string _body = "body";
        private string _severity = "severity";
        private string _subHeading = "subHeading";
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private string _slug = "slug";
        private SystemProperties _sys = new SystemProperties
        {
            ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } }
        };

        public Alert Build()
        {
            return new Alert(_title, _subHeading, _body, _severity, _sunriseDate, _sunsetDate, _slug);
        }
    }
}