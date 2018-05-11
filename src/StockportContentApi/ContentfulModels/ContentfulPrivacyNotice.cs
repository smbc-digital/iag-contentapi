using Contentful.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulPrivacyNotice
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public bool OutsideEu { get; set; }
        public bool AutomatedDecision { get; set; }
        public string Purpose { get; set; }
        public string TypeOfData { get; set; }
        public string Legislation { get; set; }
        public string Obtained { get; set; }
        public string ExternallyShared { get; set; }
        public string RetentionPeriod { get; set; }
        public string UrlOne { get; set; }
        public string UrlTwo { get; set; }
        public string UrlThree { get; set; }
        public SystemProperties Sys { get; set; }

        public ContentfulPrivacyNotice() { }

        public ContentfulPrivacyNotice(string slug, string title, string category, bool outsideEu, bool automatedDecision, string purpose, string typeOfData, string legislation, string obtained, string externallyShared, string retentionPeriod, string urlOne, string urlTwo, string urlThree, SystemProperties sys)
        {
            Slug = slug;
            Title = title;
            Category = category;
            OutsideEu = outsideEu;
            AutomatedDecision = automatedDecision;
            Purpose = purpose;
            TypeOfData = typeOfData;
            Legislation = legislation;
            Obtained = obtained;
            ExternallyShared = externallyShared;
            RetentionPeriod = retentionPeriod;
            UrlOne = urlOne;
            UrlTwo = urlTwo;
            UrlThree = urlThree;
            Sys = sys;
        }
    }
}
