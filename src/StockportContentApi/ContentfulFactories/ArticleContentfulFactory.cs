using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;

namespace StockportContentApi.ContentfulFactories
{
    public class ArticleContentfulFactory : IContentfulFactory<ContentfulArticle, Article>
    {
        private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IVideoRepository _videoRepository;

        public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory, 
            IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, 
            IContentfulFactory<ContentfulProfile, Profile> profileFactory, 
            IContentfulFactory<ContentfulTopic, Topic> topicFactory,
            IContentfulFactory<Asset, Document> documentFactory,
            IVideoRepository videoRepository)
        {
            _sectionFactory = sectionFactory;
            _crumbFactory = crumbFactory;
            _profileFactory = profileFactory;
            _topicFactory = topicFactory;
            _documentFactory = documentFactory;
            _videoRepository = videoRepository;
        }

        public Article ToModel(ContentfulArticle entry)
        {
            var sections = entry.Sections.Select(section => _sectionFactory.ToModel(section.Fields)).ToList();
            var breadcrumbs = entry.Breadcrumbs.Select(crumb => _crumbFactory.ToModel(crumb)).ToList();
            var profiles = entry.Profiles.Select(profile => _profileFactory.ToModel(profile.Fields)).ToList();
            var topic = _topicFactory.ToModel(entry.ParentTopic.Fields);
            var documents = entry.Documents.Select(document => _documentFactory.ToModel(document)).ToList();
            var body = _videoRepository.Process(entry.Body);
            var alerts = entry.Alerts.Select(alert => alert.Fields);
            var liveChat = entry.LiveChat.Fields;
            var backgroundImage = entry.BackgroundImage.File.Url;

            return new Article(body, entry.Slug, entry.Title, entry.Teaser, entry.Icon, backgroundImage, 
                sections, breadcrumbs, alerts, profiles, topic, documents, entry.SunriseDate, entry.SunsetDate, 
                entry.LiveChatVisible, liveChat);
        }
    }
}