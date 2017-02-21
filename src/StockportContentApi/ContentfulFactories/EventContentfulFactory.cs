using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class EventContentfulFactory : IContentfulFactory<ContentfulEvent, Event>
    {
        private readonly IContentfulFactory<Asset, Document> _documentFactory;

        public EventContentfulFactory(IContentfulFactory<Asset, Document> documentFactory)
        {
            _documentFactory = documentFactory;
        }

        public Event ToModel(ContentfulEvent entry)
        {
            var eventDocuments = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                                .Select(document => _documentFactory.ToModel(document)).ToList();
            var imageUrl = ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) ? entry.Image.File.Url : string.Empty;

            return new Event(entry.Title, entry.Slug, entry.Teaser, imageUrl, entry.Description, entry.Fee, entry.Location, 
                entry.SubmittedBy, entry.EventDate, entry.StartTime, entry.EndTime, entry.Occurences, entry.Frequency, entry.Breadcrumbs,
                ImageConverter.ConvertToThumbnail(imageUrl), eventDocuments, entry.Categories, entry.MapPosition, entry.BookingInformation);
        }
    }
}