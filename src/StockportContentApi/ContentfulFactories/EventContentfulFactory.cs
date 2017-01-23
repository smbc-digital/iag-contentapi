using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class EventContentfulFactory : IContentfulFactory<ContentfulEvent, Event>
    {
        private IContentfulFactory<Asset, Document> _documentFactory;

        public EventContentfulFactory(IContentfulFactory<Asset, Document> documentFactory)
        {
            _documentFactory = documentFactory;
        }

        public Event ToModel(ContentfulEvent entry)
        {
            var eventDocuments = entry.Documents.Select(document => _documentFactory.ToModel(document)).ToList();

            return new Event(entry.Title, entry.Slug, entry.Teaser, entry.Image.File.Url, entry.Description, entry.Fee, entry.Location, 
                entry.SubmittedBy, entry.EventDate, entry.StartTime, entry.EndTime, entry.Occurences, entry.Frequency, entry.Breadcrumbs,
                ImageConverter.ConvertToThumbnail(entry.Image.File.Url), eventDocuments, entry.Categories);
        }
    }
}