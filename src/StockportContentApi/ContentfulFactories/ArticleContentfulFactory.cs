using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ArticleContentfulFactory : IContentfulFactory<ContentfulArticle, Article>
    {
        private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Topic> _parentTopicFactory;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IVideoRepository _videoRepository;

        public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory, 
            IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, 
            IContentfulFactory<ContentfulProfile, Profile> profileFactory, 
            IContentfulFactory<ContentfulTopic, Topic> topicFactory,
            IContentfulFactory<Entry<ContentfulCrumb>, Topic> parentTopicFactory,
            IContentfulFactory<Asset, Document> documentFactory,
            IVideoRepository videoRepository)
        {
            _sectionFactory = sectionFactory;
            _crumbFactory = crumbFactory;
            _profileFactory = profileFactory;
            _topicFactory = topicFactory;
            _documentFactory = documentFactory;
            _videoRepository = videoRepository;
            _parentTopicFactory = parentTopicFactory;
        }

        public Article ToModel(ContentfulArticle entry)
        {
            var sections = entry.Sections.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                         .Select(section => _sectionFactory.ToModel(section.Fields)).ToList();
            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var profiles = entry.Profiles.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                         .Select(profile => _profileFactory.ToModel(profile.Fields)).ToList();

            var topicInBreadcrumb = entry.Breadcrumbs.LastOrDefault(o => o.SystemProperties.ContentType.SystemProperties.Id == "topic");

            var topic = topicInBreadcrumb != null
                ? ContentfulHelpers.EntryIsNotALink(topicInBreadcrumb.SystemProperties)
                    ? _parentTopicFactory.ToModel(topicInBreadcrumb)
                    : new NullTopic()
                : new NullTopic();

            var documents = entry.Documents.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                           .Select(document => _documentFactory.ToModel(document)).ToList();
            var body = _videoRepository.Process(entry.Body);
            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                     .Select(alert => alert.Fields);
            var liveChat = ContentfulHelpers.EntryIsNotALink(entry.LiveChat.SystemProperties) 
                                ? entry.LiveChat.Fields : new NullLiveChat();
            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties) 
                                        ? entry.BackgroundImage.File.Url : string.Empty;
            var image = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                                        ? entry.Image.File.Url : string.Empty;

            return new Article(body, entry.Slug, entry.Title, entry.Teaser, entry.Icon, backgroundImage, image,
                sections,breadcrumbs, alerts, profiles, topic, documents, entry.SunriseDate, entry.SunsetDate, 
                entry.LiveChatVisible, liveChat);
        }
    }
}