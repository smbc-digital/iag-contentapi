using System.Linq;
using Contentful.Core.Models;
using Microsoft.AspNetCore.Http;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApi.ContentfulFactories
{
    public class DocumentPageContentfulFactory : IContentfulFactory<ContentfulDocumentPage, DocumentPage>
    {
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly DateComparer _dateComparer;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public DocumentPageContentfulFactory(
            IContentfulFactory<Asset, Document> documentFactory,
            IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
            IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
            ITimeProvider timeProvider,
            IHttpContextAccessor httpContextAccessor)
        {
            _documentFactory = documentFactory;
            _subitemFactory = subitemFactory;
            _crumbFactory = crumbFactory;
            _dateComparer = new DateComparer(timeProvider);
            _httpContextAccessor = httpContextAccessor;
        }

        public DocumentPage ToModel(ContentfulDocumentPage entryContentfulDocumentPage)
        {
            var entry = entryContentfulDocumentPage;

            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                               .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            var documents = entry.Documents.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                                           .Select(document => _documentFactory.ToModel(document)).ToList();

            var relatedDocuments = entry.RelatedDocuments.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                    .Select(item => _subitemFactory.ToModel(item)).ToList();

            var updatedAt = entry.Sys.UpdatedAt.Value;

            return new DocumentPage(
                title: entry.Title,
                slug: entry.Slug,
                teaser: entry.Teaser,
                metaDescription: entry.MetaDescription,
                aboutThisDocument: entry.AboutThisDocument,
                documents: documents,
                requestAnAccessibleFormatContactInformation: entry.RequestAnAccessibleFormatContactInformation,
                furtherInformation: entry.FurtherInformation,
                relatedDocuments: relatedDocuments,
                datePublished: entry.DatePublished,
                lastUpdated: entry.LastUpdated,
                breadcrumbs: breadcrumbs,
                updatedAt: updatedAt)
                .StripData(_httpContextAccessor);
        }
    }
}