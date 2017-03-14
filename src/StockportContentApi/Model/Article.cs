using System;
using System.Collections.Generic;
using System.Linq;

namespace StockportContentApi.Model
{
    public class Article
    {
        public string Body { get; set; }
        public string Slug { get; }
        public string Title { get; }
        public string Teaser { get; }
        public string Icon { get; }
        public string BackgroundImage { get; }
        public string Image { get; }
        public List<Section> Sections { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public IEnumerable<Alert> Alerts { get; }
        public IEnumerable<Alert> AlertsInline { get; }
        public IEnumerable<Profile> Profiles { get; }
        public Topic ParentTopic { get; }
        public List<Document> Documents { get; }
        public DateTime SunriseDate { get; }
        public DateTime SunsetDate { get; }
        public bool LiveChatVisible { get; }
        public LiveChat LiveChat { get; }

        public Article(string body, string slug, string title, string teaser, string icon, string backgroundImage, string image, List<Section> sections,
            IEnumerable<Crumb> breadcrumbs, IEnumerable<Alert> alerts, IEnumerable<Profile> profiles, Topic parentTopic, List<Document> documents,
            DateTime sunriseDate, DateTime sunsetDate, bool liveChatVisible, LiveChat liveChat, IEnumerable<Alert> alertsInline)
        {
            Body = body;
            Slug = slug;
            Title = title;
            Teaser = teaser;
            Icon = icon;
            BackgroundImage = backgroundImage;
            Image = image;
            Sections = sections;
            Breadcrumbs = breadcrumbs;
            Alerts = alerts;
            Profiles = profiles;
            ParentTopic = parentTopic;
            Documents = documents;
            SunriseDate = sunriseDate;
            SunsetDate = sunsetDate;
            LiveChatVisible = liveChatVisible;
            LiveChat = liveChat;
            AlertsInline = alertsInline;
        }

        public void ReplaceSection(Section oldSection, Section newSection)
        {
            var foundSection = Sections.SingleOrDefault(s => s.Slug == oldSection.Slug);
            Ensure.ArgumentNotNull(foundSection, "section");

            var sectionList = Sections.ToList();
            sectionList.Remove(foundSection);
            sectionList.Add(newSection);
            Sections = sectionList;
        }
    }

    public class NullArticle : Article
    {
        public NullArticle()
        : base(
            string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, new List<Section>(), new List<Crumb>(),
            new List<Alert>(), new List<Profile>(), new NullTopic(), new List<Document>(), new DateTime(), new DateTime(), false, new NullLiveChat(), new List<Alert>())
        { }
    }
}