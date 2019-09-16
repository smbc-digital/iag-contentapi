using System;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulAlertBuilder
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
            ContentType = new ContentType {SystemProperties = new SystemProperties {Id = "id"}},
            Id = "id"
        };

        public ContentfulAlert Build()
        {
            return new ContentfulAlert
            {
                Title = _title,
                Body = _body,
                Severity = _severity,
                SubHeading = _subHeading,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                Sys = _sys,
                Slug = _slug
            };
        }

        public ContentfulAlertBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulAlertBuilder Body(string body)
        {
            _body = body;
            return this;
        }

        public ContentfulAlertBuilder Severity(string severity)
        {
            _severity = severity;
            return this;
        }

        public ContentfulAlertBuilder SeverSubHeadingity(string subHeading)
        {
            _subHeading = subHeading;
            return this;
        }

        public ContentfulAlertBuilder SunriseDate(DateTime sunriseDate)
        {
            _sunriseDate = sunriseDate;
            return this;
        }

        public ContentfulAlertBuilder SunsetDate(DateTime sunsetDate)
        {
            _sunsetDate = sunsetDate;
            return this;
        }
    }
}
