using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApi.ContentfulFactories.ArticleFactories
{
    public class ArticleContentfulFactory : IContentfulFactory<ContentfulArticle, Article>
    {
        private readonly IContentfulFactory<ContentfulSection, Section> _sectionFactory;
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IContentfulFactory<ContentfulArticle, Topic> _parentTopicFactory;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IVideoRepository _videoRepository;
        private readonly DateComparer _dateComparer;

        public ArticleContentfulFactory(IContentfulFactory<ContentfulSection, Section> sectionFactory,
            IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
            IContentfulFactory<ContentfulProfile, Profile> profileFactory,
            IContentfulFactory<ContentfulArticle, Topic> parentTopicFactory,
            IContentfulFactory<Asset, Document> documentFactory,
            IVideoRepository videoRepository,
            ITimeProvider timeProvider,
            IContentfulFactory<ContentfulAlert, Alert> alertFactory)
        {
            _sectionFactory = sectionFactory;
            _crumbFactory = crumbFactory;
            _profileFactory = profileFactory;
            _documentFactory = documentFactory;
            _videoRepository = videoRepository;
            _parentTopicFactory = parentTopicFactory;
            _dateComparer = new DateComparer(timeProvider);
            _alertFactory = alertFactory;
        }

        public Article ToModel(ContentfulArticle entryContentfulArticle)
        {
            var entry = entryContentfulArticle;

            var sections = entry.Sections.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys) && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                         .Select(section => _sectionFactory.ToModel(section)).ToList();
            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var profiles = entry.Profiles.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                         .Select(profile => _profileFactory.ToModel(profile)).ToList();

            var topic = _parentTopicFactory.ToModel(entryContentfulArticle) ?? new NullTopic();

            var documents = entry.Documents.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                           .Select(document => _documentFactory.ToModel(document)).ToList();

            var body = !string.IsNullOrEmpty(entry.Body) ? _videoRepository.Process(entry.Body) : string.Empty;

            var alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                     .Select(alert => _alertFactory.ToModel(alert));

            var alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                     .Select(alertInline => _alertFactory.ToModel(alertInline));

            var backgroundImage = ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                                        ? entry.BackgroundImage.File.Url : string.Empty;
            var image = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                                        ? entry.Image.File.Url : string.Empty;

            var updatedAt = entry.Sys.UpdatedAt.Value;

            return new Article(body, entry.Slug, entry.Title, entry.Teaser, entry.MetaDescription, entry.Icon, backgroundImage, image,
                sections, breadcrumbs, alerts, profiles, topic, documents, entry.SunriseDate, entry.SunsetDate, alertsInline, updatedAt);
        }
    }
}