using System.Collections.Generic;
using System.Linq;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApi.ContentfulFactories.EventFactories
{
    public class EventContentfulFactory : IContentfulFactory<ContentfulEvent, Event>
    {
        private readonly IContentfulFactory<Asset, Document> _documentFactory;

        private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;
        private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
        private readonly IContentfulFactory<ContentfulEventCategory, EventCategory> _eventCategoryFactory;
        private readonly DateComparer _dateComparer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public EventContentfulFactory(IContentfulFactory<Asset, Document> documentFactory, IContentfulFactory<ContentfulGroup, Group> groupFactory, IContentfulFactory<ContentfulEventCategory, EventCategory> eventCategoryFactory, IContentfulFactory<ContentfulAlert, Alert> alertFactory, ITimeProvider timeProvider, IHttpContextAccessor httpContextAccessor)
        {
            _documentFactory = documentFactory;
            _groupFactory = groupFactory;
            _alertFactory = alertFactory;
            _eventCategoryFactory = eventCategoryFactory;
            _dateComparer = new DateComparer(timeProvider);
            _httpContextAccessor = httpContextAccessor;
        }

        public Event ToModel(ContentfulEvent entry)
        {
            var eventDocuments =
                entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                    .Select(document => _documentFactory.ToModel(document)).ToList();

            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties)
                ? entry.Image.File.Url
                : string.Empty; 

            var group = _groupFactory.ToModel(entry.Group);

            var categories = entry.EventCategories.Select(ec => _eventCategoryFactory.ToModel(ec));

            var alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                                        &&
                                                        _dateComparer.DateNowIsWithinSunriseAndSunsetDates(
                                                            alert.SunriseDate, alert.SunsetDate))
                .Select(alert => _alertFactory.ToModel(alert)).ToList();

            return new Event(entry.Title, entry.Slug, entry.Teaser, imageUrl, entry.Description, entry.Fee,
                entry.Location,
                entry.SubmittedBy, entry.EventDate, entry.StartTime, entry.EndTime, entry.Occurences, entry.Frequency,
                new List<Crumb> { new Crumb("Events", string.Empty, "events") },
                ImageConverter.ConvertToThumbnail(imageUrl), eventDocuments, entry.Categories, entry.MapPosition,
                entry.Featured, entry.BookingInformation, entry.Sys.UpdatedAt, entry.Tags, group, alerts, categories.ToList(), entry.Free, entry.Paid, entry.AccessibleTransportLink).StripData(_httpContextAccessor);
        }
    }
}