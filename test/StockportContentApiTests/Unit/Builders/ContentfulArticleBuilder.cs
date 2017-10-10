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
        private LiveChat _liveChat = new LiveChat("title", "text") {Sys = new SystemProperties() { Type = "Entry" } };
        private Asset _backgroundImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private string _body = "body";
        private DateTime _sunriseDate = new DateTime(2016, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(2017, 1, 20, 0, 0, 0, DateTimeKind.Utc);
        private ContentfulLiveChat _contentfulLiveChat = new ContentfulLiveChatBuilder().Build();

        private List<ContentfulAlert> _alertsInline = new List<ContentfulAlert>
        {
            new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
                Sys = new SystemProperties() {Type = "Entry"},
                Slug = "slug"
            }
        };

        private List<ContentfulReference> _breadcrumbs = new List<ContentfulReference>
        {
            new ContentfulReferenceBuilder().Build()
        };

        private List<ContentfulAlert> _alerts = new List<ContentfulAlert>
        {
            new ContentfulAlert()
            {
                Title = "title",
                SubHeading = "subHeading",
                Body = "body",
                Severity = "severity",
                SunriseDate = new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                SunsetDate = new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc),
                Sys = new SystemProperties() {Type = "Entry"},
                Slug = "slug"
            }
        };

        private List<Asset> _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
        private bool _liveChatVisible = false;
        private ContentfulTopic _topic = new ContentfulParentTopicBuilder().SystemId("topic").SystemContentTypeId("topic").Build();
        private List<ContentfulProfile> _profiles = new List<ContentfulProfile> {
                                    new ContentfulProfileBuilder().Build() };
        private List<ContentfulSection> _sections = new List<ContentfulSection>{
                                   (new ContentfulSectionBuilder().Build()) };
        private string _systemId = "id";
        private string _contentTypeSystemId = "id";

        public ContentfulArticle Build()
        {
            return new ContentfulArticle
            {
                Alerts = _alerts,
                Advertisement = new ContentfulAdvertisement(),
                BackgroundImage = _backgroundImage,
                Body = _body,
                Breadcrumbs = _breadcrumbs,
                Documents = _documents,
                Icon = _icon,
                LiveChatText = _contentfulLiveChat,
                LiveChatVisible = _liveChatVisible,
                Profiles = _profiles,
                Slug = _slug,
                Title = _title,
                Teaser = _teaser,
                Sections = _sections,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                Image = _image,
                AlertsInline = _alertsInline,
                Sys = new SystemProperties
                {
                    ContentType = new ContentType { SystemProperties = new SystemProperties { Id = _contentTypeSystemId } },
                    Id = _systemId

                }
        };
        }

        public ContentfulArticleBuilder Slug(string slug)
        {
            _slug = slug;
            return this;
        }

        public ContentfulArticleBuilder Title(string title)
        {
            _title = title;
            return this;
        }

        public ContentfulArticleBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
        {
            _breadcrumbs = breadcrumbs;
            return this;
        }

        public ContentfulArticleBuilder WithOutSection()
        {
            _sections = new List<ContentfulSection>();
            return this;
        }

        public ContentfulArticleBuilder AlertsInline(List<ContentfulAlert> alertsInline)
        {
            _alertsInline = alertsInline;
            return this;
        }

        public ContentfulArticleBuilder Alerts(List<ContentfulAlert> alerts)
        {
            _alerts = alerts;
            return this;
        }

        public ContentfulArticleBuilder Body(string body)
        {
            _body = body;
            return this;
        }

        public ContentfulArticleBuilder SystemId(string id)
        {
            _systemId = id;
            return this;
        }

        public ContentfulArticleBuilder SystemContentTypeId(string id)
        {
            _contentTypeSystemId = id;
            return this;
        }
    }
}
