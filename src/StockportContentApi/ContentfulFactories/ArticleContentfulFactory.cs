﻿using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ArticleContentfulFactory : IContentfulFactory<Entry<ContentfulArticle>, Article>
    {
        private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;
        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IContentfulFactory<Entry<ContentfulArticle>, Topic> _parentTopicFactory;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IVideoRepository _videoRepository;
        private readonly DateComparer _dateComparer;

        public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory, 
            IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory, 
            IContentfulFactory<ContentfulProfile, Profile> profileFactory, 
            IContentfulFactory<Entry<ContentfulArticle>, Topic> parentTopicFactory,
            IContentfulFactory<Asset, Document> documentFactory,
            IVideoRepository videoRepository,
            ITimeProvider timeProvider)
        {
            _sectionFactory = sectionFactory;
            _crumbFactory = crumbFactory;
            _profileFactory = profileFactory;
            _documentFactory = documentFactory;
            _videoRepository = videoRepository;
            _parentTopicFactory = parentTopicFactory;
            _dateComparer = new DateComparer(timeProvider);
        }

        public Article ToModel(Entry<ContentfulArticle> entryContentfulArticle)
        {
            var entry = entryContentfulArticle.Fields;

            var sections = entry.Sections.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.Fields.SunriseDate, section.Fields.SunsetDate))
                                         .Select(section => _sectionFactory.ToModel(section.Fields)).ToList();
            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var profiles = entry.Profiles.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                         .Select(profile => _profileFactory.ToModel(profile.Fields)).ToList();

            var topic = _parentTopicFactory.ToModel(entryContentfulArticle) ?? new NullTopic();

            var documents = entry.Documents.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                           .Select(document => _documentFactory.ToModel(document)).ToList();

            var body = !string.IsNullOrEmpty(entry.Body) ? _videoRepository.Process(entry.Body) : string.Empty;

            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.Fields.SunriseDate, section.Fields.SunsetDate))
                                     .Select(alert => alert.Fields);

            var alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.Fields.SunriseDate, section.Fields.SunsetDate))
                                     .Select(alertInline => alertInline.Fields);

            var liveChat = ContentfulHelpers.EntryIsNotALink(entry.LiveChatText.SystemProperties) 
                                ? entry.LiveChatText.Fields : new NullLiveChat();
            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties) 
                                        ? entry.BackgroundImage.File.Url : string.Empty;
            var image = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                                        ? entry.Image.File.Url : string.Empty;

            return new Article(body, entry.Slug, entry.Title, entry.Teaser, entry.Icon, backgroundImage, image,
                sections,breadcrumbs, alerts, profiles, topic, documents, entry.SunriseDate, entry.SunsetDate, 
                entry.LiveChatVisible, liveChat, alertsInline);
        }
    }
}