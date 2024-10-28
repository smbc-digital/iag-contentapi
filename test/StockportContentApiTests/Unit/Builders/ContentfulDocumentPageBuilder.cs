namespace StockportContentApiTests.Unit.Builders;

public class ContentfulDocumentPageBuilder
{
    private readonly string _slug = "slug";
    private readonly string _aboutTheDocument = "about the document";
    private readonly List<Asset> _documents = new List<Asset>()
    {
        new Asset()
        {
            SystemProperties = new SystemProperties()
        }
    };
    private readonly string _awsDocuments = "url.pdf";
    private readonly string _requestAnAccessibleFormatContactInformation = "Request An Accessible Format Contact Information";
    private readonly string _furtherInformation = "further information";
    private readonly List<ContentfulReference> _relatedDocuments = new()
    {
        new ContentfulReferenceBuilder().Build()
    };
    private readonly DateTime _datePublished = new(2016, 10, 05, 00, 00, 00, DateTimeKind.Utc);
    private readonly DateTime _lastUpdated = new(2018, 10, 05, 00, 00, 00, DateTimeKind.Utc);
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
            Slug = _slug,
            AboutTheDocument = _aboutTheDocument,
            Documents = _documents,
            AwsDocuments = _awsDocuments,
            RequestAnAccessibleFormatContactInformation = _requestAnAccessibleFormatContactInformation,
            FurtherInformation = _furtherInformation,
            RelatedDocuments = _relatedDocuments,
            DatePublished = _datePublished,
            LastUpdated = _lastUpdated,
            Breadcrumbs = _breadcrumbs,
            Sys = _sys
        };
}