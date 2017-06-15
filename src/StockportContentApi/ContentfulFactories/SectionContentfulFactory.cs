using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class SectionContentfulFactory : IContentfulFactory<ContentfulSection, Section>
    {
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IVideoRepository _videoRepository;
        private readonly DateComparer _dateComparer;
        private IContentfulFactory<ContentfulAlert, Alert> _alertFactory;

        public SectionContentfulFactory(IContentfulFactory<ContentfulProfile, Profile> profileFactory, 
                                        IContentfulFactory<Asset, Document> documentFactory,
                                        IVideoRepository videoRepository,
                                        ITimeProvider timeProvider,
                                        IContentfulFactory<ContentfulAlert, Alert> alertFactory )
        {
            _profileFactory = profileFactory;
            _documentFactory = documentFactory;
            _videoRepository = videoRepository;
            _dateComparer = new DateComparer(timeProvider);
            _alertFactory = alertFactory;
        }

        public Section ToModel(ContentfulSection entry)
        {
            var profiles = entry.Profiles.Where(profile => ContentfulHelpers.EntryIsNotALink(profile.Sys))
                                         .Select(profile => _profileFactory.ToModel(profile)).ToList();
            var documents = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                           .Select(document => _documentFactory.ToModel(document)).ToList();
            var body = _videoRepository.Process(entry.Body);

            var alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                     .Select(alertInline => _alertFactory.ToModel(alertInline));

            return new Section(entry.Title, entry.Slug, body, profiles, documents, 
                               entry.SunriseDate, entry.SunsetDate, alertsInline);
        }
    }
}