using System;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class DocumentPage
    {
        public string Title { get; set; }
        public string Slug { get; }
        public string Teaser { get; }
        public string MetaDescription { get; }
        public string AboutThisDocument { get; }
        public List<Document> Documents { get; }
        public string RequestAnAccessibleFormatContactInformation { get;}
        public string FurtherInformation { get;}
        public List<SubItem> RelatedDocuments { get; }
        public DateTime DatePublished { get; }
        public DateTime LastUpdated { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public DateTime UpdatedAt { get; }
        
        public DocumentPage(
            string title,
            string slug,
            string teaser,
            string metaDescription,
            string aboutThisDocument,
            List<Document> documents,
            string requestAnAccessibleFormatContactInformation,
            string furtherInformation,
            List<SubItem> relatedDocuments,
            DateTime datePublished,
            DateTime lastUpdated,
            IEnumerable<Crumb> breadcrumbs,
            DateTime updatedAt)
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            MetaDescription = metaDescription;
            AboutThisDocument = aboutThisDocument;
            Documents = documents;
            RequestAnAccessibleFormatContactInformation = requestAnAccessibleFormatContactInformation;
            FurtherInformation = furtherInformation;
            RelatedDocuments = relatedDocuments;
            DatePublished = datePublished;
            LastUpdated = lastUpdated;
            Breadcrumbs = breadcrumbs;
            UpdatedAt = updatedAt;
        }
    }

    public class NullDocumentPage : DocumentPage
    {
        public NullDocumentPage()
        : base(
            title: string.Empty,
            slug: string.Empty,
            teaser: string.Empty,
            metaDescription: string.Empty,
            aboutThisDocument: string.Empty,
            documents: new List<Document>(),
            requestAnAccessibleFormatContactInformation: string.Empty,
            furtherInformation: string.Empty,
            relatedDocuments: new List<SubItem>(),
            datePublished: new DateTime(),
            lastUpdated: new DateTime(),
            breadcrumbs: new List<Crumb>(),
            updatedAt: new DateTime())
        { }
    }
}