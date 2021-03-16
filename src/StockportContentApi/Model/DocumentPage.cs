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
        public string AboutTheDocument { get; }
        public List<Document> Documents { get; }
        public string AwsDocuments { get; }
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
            string aboutTheDocument,
            List<Document> documents,
            string awsDocuments,
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
            AboutTheDocument = aboutTheDocument;
            Documents = documents;
            AwsDocuments = awsDocuments;
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
            aboutTheDocument: string.Empty,
            documents: new List<Document>(),
            awsDocuments: string.Empty,
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