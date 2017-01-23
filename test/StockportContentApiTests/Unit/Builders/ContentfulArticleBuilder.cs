using System;
using System.Collections.Generic;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApiTests.Unit.Builders
{
    public class ContentfulArticleBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _teaser = "teaser";
        private string _icon = "icon";
        private LiveChat _liveChat = new LiveChat("title", "text");
        private Asset _backgroundImage = new Asset { File = new File { Url = "image-url.jpg" } };
        private string _body = "body";
        private DateTime _sunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc);
        private List<Entry<ContentfulCrumb>> _breadcrumbs = new List<Entry<ContentfulCrumb>>
        {
            new Entry<ContentfulCrumb>() {Fields = new ContentfulCrumbBuilder().Build(),
                SystemProperties = new SystemProperties() { ContentType = new ContentType { SystemProperties = new SystemProperties { Id = "id" } } }}
        };
        private List<Alert> _alerts = new List<Alert> { new Alert("title", "subHeading", "body", 
                                                                 "severity", new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc), 
                                                                 new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc)) };
        private List<Asset> _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
        private bool _liveChatVisible = false;
        private ContentfulTopic _topic = new ContentfulTopicBuilder().Build();
        private List<ContentfulProfile> _profiles = new List<ContentfulProfile> { new ContentfulProfileBuilder().Build() };
        private List<ContentfulSection> _sections = new List<ContentfulSection> { new ContentfulSectionBuilder().Build() };

        public ContentfulArticle Build()
        {
            return new ContentfulArticle
            {
                Alerts = _alerts,
                BackgroundImage = _backgroundImage,
                Body = _body,
                Breadcrumbs = _breadcrumbs,
                Documents = _documents,
                Icon = _icon,
                LiveChat = _liveChat,
                LiveChatVisible = _liveChatVisible,
                ParentTopic = _topic,
                Profiles = _profiles,
                Slug = _slug,
                Title = _title,
                Teaser = _teaser,
                Sections = _sections,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate
            };
        }
    }
}
