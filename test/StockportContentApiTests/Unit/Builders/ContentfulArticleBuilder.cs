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
        private Entry<LiveChat> _liveChat = new ContentfulEntryBuilder<LiveChat>().Fields(new LiveChat("title", "text")).Build();
        private Asset _backgroundImage = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private Asset _image = new ContentfulAssetBuilder().Url("image-url.jpg").Build();
        private string _body = "body";
        private DateTime _sunriseDate = new DateTime(2016, 1, 10, 0, 0, 0, DateTimeKind.Utc);
        private DateTime _sunsetDate = new DateTime(2017, 1, 20, 0, 0, 0, DateTimeKind.Utc);

        private List<Entry<ContentfulCrumb>> _breadcrumbs = new List<Entry<ContentfulCrumb>>
        {
            new ContentfulEntryBuilder<ContentfulCrumb>().Fields(new ContentfulCrumbBuilder().Build()).ContentTypeSystemId("topic").Build()
        };

        private List<Entry<Alert>> _alerts = new List<Entry<Alert>>
            { new ContentfulEntryBuilder<Alert>().Fields(new Alert("title", "subHeading", "body", "severity",
                                                         new DateTime(0001, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                                                         new DateTime(9999, 9, 9, 0, 0, 0, DateTimeKind.Utc))).Build() };
        private List<Asset> _documents = new List<Asset> { new ContentfulDocumentBuilder().Build() };
        private bool _liveChatVisible = false;
        private Entry<ContentfulTopic> _topic = new ContentfulEntryBuilder<ContentfulTopic>().Fields(new ContentfulParentTopicBuilder().Build()).Build();
        private List<Entry<ContentfulProfile>> _profiles = new List<Entry<ContentfulProfile>> {
                                    new ContentfulEntryBuilder<ContentfulProfile>().Fields(new ContentfulProfileBuilder().Build()).Build() };
        private List<Entry<ContentfulSection>> _sections = new List<Entry<ContentfulSection>> {
                                    new ContentfulEntryBuilder<ContentfulSection>().Fields(new ContentfulSectionBuilder().Build()).Build() };

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
                LiveChatText = _liveChat,
                LiveChatVisible = _liveChatVisible,
                ParentTopic = _topic,
                Profiles = _profiles,
                Slug = _slug,
                Title = _title,
                Teaser = _teaser,
                Sections = _sections,
                SunriseDate = _sunriseDate,
                SunsetDate = _sunsetDate,
                Image = _image
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

        public ContentfulArticleBuilder Breadcrumbs(List<Entry<ContentfulCrumb>> breadcrumbs)
        {
            _breadcrumbs = breadcrumbs;
            return this;
        }

        public ContentfulArticleBuilder WithOutSection()
        {
            _sections = new List<Entry<ContentfulSection>>();
            return this;
        }
    }
}
