using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulNewsBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _teaser = "teaser";
        private string _imageUrl = "image-url.jpg";
        private string _body = "body";
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private List<Crumb> _breadcrumbs = new List<Crumb> { new Crumb("News", string.Empty, "news") };
        private List<string> _tags = new List<string> {"tag"};
        private List<Alert> _alerts = new List<Alert> {new Alert("title", "subHeading", "body", 
                                                                 "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc), 
                                                                 new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)) };
        private List<Asset> _documents = new List<Asset>
        {
            new Asset
            {
                Description = "documentTitle",
                File = new Contentful.Core.Models.File()
                {
                    Details = new FileDetails {Size = 674192},
                    Url = "url.pdf",
                    FileName = "fileName"
                },
                SystemProperties =
                    new SystemProperties { UpdatedAt = new DateTime(2016, 10, 05, 00, 00, 00, DateTimeKind.Utc) }
            }
        };
        private List<string> _categories = new List<string> {"category"};

        public ContentfulNews Build()
        {
            return new ContentfulNews
            {
                Title = _title,
                Slug = _slug,
                Teaser = _teaser,
                Image = new Asset { File = new File { Url = _imageUrl} },
                Body = _body,
                SunriseDate  = _sunriseDate,
                SunsetDate  = _sunsetDate,
                Breadcrumbs  = _breadcrumbs,
                Tags = _tags,
                Alerts  = _alerts,
                Documents  = _documents,
                Categories  = _categories
            };
        }

        public ContentfulNewsBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }
    }
}
