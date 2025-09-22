namespace StockportContentApiTests.Unit.Builders;

public class ContentfulDocumentPageBuilder
{
    private readonly List<Asset> _documents = new()
    {
        new Asset()
        {
            SystemProperties = new SystemProperties()
        }
    };

    private readonly List<ContentfulReference> _relatedDocuments = new()
    {
        new ContentfulReferenceBuilder().Build()
    };

    private readonly List<ContentfulReference> _breadcrumbs = new()
    {
        new ContentfulReferenceBuilder().Build()
    };
    
    private readonly SystemProperties _sys = new()
    {
        UpdatedAt = new DateTime()
    };

    public ContentfulDocumentPage Build()
        => new ()
        {
            Slug = "slug",
            Title = "title",
            AboutTheDocument = "about the document",
            Documents = _documents,
            AwsDocuments = "url.pdf",
            RequestAnAccessibleFormatContactInformation = "Request An Accessible Format Contact Information",
            FurtherInformation = "further information",
            RelatedDocuments = _relatedDocuments,
            DatePublished = new(2016, 10, 05, 00, 00, 00, DateTimeKind.Utc),
            LastUpdated = new(2018, 10, 05, 00, 00, 00, DateTimeKind.Utc),
            Breadcrumbs = _breadcrumbs,
            Sys = _sys
        };
}