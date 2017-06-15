using System;
using Contentful.Core.Configuration;
using Contentful.Core.Models;
using Newtonsoft.Json;

namespace StockportContentApi.Model
{
    public class Alert
    {
        public string Title { get; }
        public string SubHeading { get; }
        public string Body { get; }
        public string Severity { get; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }

        public Alert(string title, string subHeading, string body, string severity, DateTime sunriseDate,
            DateTime sunsetDate)
        {
            Title = title;
            SubHeading = subHeading;
            Body = body;
            Severity = severity;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;          
        }
    }

    public class NullAlert : Alert
    {
        public NullAlert() : base(string.Empty,string.Empty,string.Empty,string.Empty, DateTime.MinValue, DateTime.MinValue) { }
    }

}
