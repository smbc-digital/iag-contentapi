using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class EventContentfulFactory : IContentfulFactory<ContentfulEvent, Event>
    {
        public Event ToModel(ContentfulEvent entry)
        {
            var eventDocuments = entry.Documents.Select(
                document =>
                    new Document(document.Description,
                        (int)document.File.Details.Size,
                        DateComparer.DateFieldToDate(document.SystemProperties.UpdatedAt),
                        document.File.Url, document.File.FileName)).ToList();

            return new Event(entry.Title, entry.Slug, entry.Teaser, entry.Image.File.Url, entry.Description, entry.Fee, entry.Location, 
                entry.SubmittedBy, entry.EventDate, entry.StartTime, entry.EndTime, entry.Occurences, entry.Frequency, entry.Breadcrumbs,
                ImageConverter.ConvertToThumbnail(entry.Image.File.Url), eventDocuments, entry.Categories);
        }
    }
}