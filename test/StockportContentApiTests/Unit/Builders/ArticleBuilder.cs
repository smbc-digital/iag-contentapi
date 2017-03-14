using System;
using System.Collections.Generic;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ArticleBuilder
    {
        private string _slug = "slug";
        private string _title = "title";
        private string _body = "body";
        private string _teaser = "teaser";
        private string _icon = "icon";
        private string _backgroundImage = "back-image-url.jpg";
        private string _image = "image-url.jpg";
        private bool _liveChatVisible =  false;
        private List<Section> _sections = new List<Section> { new Section("title", "slug", "body", new List<Profile>(), new List<Document>(), new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)) };
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private List<Crumb> _breadcrumbs = new List<Crumb> {new Crumb("Article", "article", "article")};
        private List<Document> _documents = new List<Document> { new Document("title", 22, DateTime.MinValue, "url", "fileName") };
        private List<Alert> _alerts = new List<Alert> { new Alert("title", "subHeading", "body", "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)) };
        private List<Alert> _alertsInline = new List<Alert> { new Alert("title", "subHeading", "body", "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)) };
        private List<Profile> _profiles = new List<Profile> { new Profile("type", "title", "slug", "subtitle", "teaser", "image", "body", "icon", "background", new List<Crumb>()) };
        private Topic _parentTopic = new Topic("slug", "name", "teaser", "summary", "icon", "background", "image", new List<SubItem>(), new List<SubItem>(), new List<SubItem>(), new List<Crumb>(), 
                                                new List<Alert>(), new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc), new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc), false, "id");
        private LiveChat _liveChat = new LiveChat("title", "text");

        public Article Build()
        {
            return new Article(_body, _slug, _title, _teaser, _icon, _backgroundImage, _image, _sections, 
                _breadcrumbs, _alerts, _profiles, _parentTopic, _documents, _sunriseDate, 
                _sunsetDate, _liveChatVisible, _liveChat , _alertsInline);
        }

        public ArticleBuilder SunriseDate(DateTime sunriseDate)
        {
            _sunriseDate = sunriseDate;
            return this;
        }

        public ArticleBuilder SunsetDate(DateTime sunsetDate)
        {
            _sunsetDate = sunsetDate;
            return this;
        }
    }
}
