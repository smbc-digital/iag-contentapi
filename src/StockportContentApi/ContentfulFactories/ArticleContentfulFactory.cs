using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ArticleContentfulFactory : IContentfulFactory<ContentfulArticle, Article>
    {
        private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory;

        public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory, 
            IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, 
            IContentfulFactory<ContentfulProfile, Profile> profileFactory, 
            IContentfulFactory<ContentfulTopic, Topic> topicFactory)
        {
            _sectionFactory = sectionFactory;
            _crumbFactory = crumbFactory;
            _profileFactory = profileFactory;
            _topicFactory = topicFactory;
        }

        public Article ToModel(ContentfulArticle entry)
        {
            var sections = entry.Sections.Select(section => _sectionFactory.ToModel(section)).ToList();
            var breadcrumbs = entry.Breadcrumbs.Select(crumb => _crumbFactory.ToModel(crumb)).ToList();
            var profiles = entry.Profiles.Select(profile => _profileFactory.ToModel(profile)).ToList();
            var topic = _topicFactory.ToModel(entry.ParentTopic);
            var documents = entry.Documents.Select(
                document =>
                    new Document(document.Description,
                        (int)document.File.Details.Size,
                        DateComparer.DateFieldToDate(document.SystemProperties.UpdatedAt),
                        document.File.Url, document.File.FileName)).ToList();

            return new Article(entry.Body, entry.Slug, entry.Title, entry.Teaser, entry.Icon, entry.BackgroundImage.File.Url, 
                sections, breadcrumbs, entry.Alerts, profiles, topic, documents, entry.SunriseDate, entry.SunsetDate, 
                entry.LiveChatVisible, entry.LiveChat);
        }
    }
}