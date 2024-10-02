namespace StockportContentApi.ContentfulFactories;

public class DocumentPageContentfulFactory : IContentfulFactory<ContentfulDocumentPage, DocumentPage>
{
    private readonly IContentfulFactory<Asset, Document> _documentFactory;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
    private readonly DateComparer _dateComparer;

    public DocumentPageContentfulFactory(
        IContentfulFactory<Asset, Document> documentFactory,
        IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
        ITimeProvider timeProvider)
    {
        _documentFactory = documentFactory;
        _subitemFactory = subitemFactory;
        _crumbFactory = crumbFactory;
        _dateComparer = new DateComparer(timeProvider);
    }

    public DocumentPage ToModel(ContentfulDocumentPage entry)
    {
        if (entry is null)
            return null;

        return new()
        {
            Title = entry.Title,
            Slug = entry.Slug,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            AboutTheDocument = entry.AboutTheDocument,

            Documents = entry.Documents.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
                            .Select(document => _documentFactory.ToModel(document)).ToList(),
            
            AwsDocuments = entry.AwsDocuments,
            RequestAnAccessibleFormatContactInformation = entry.RequestAnAccessibleFormatContactInformation,
            FurtherInformation = entry.FurtherInformation,

            RelatedDocuments = entry.RelatedDocuments.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                .Select(item => _subitemFactory.ToModel(item)).ToList(),

            DatePublished = entry.DatePublished,
            LastUpdated = entry.LastUpdated,

            Breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                            .Select(crumb => _crumbFactory.ToModel(crumb)).ToList(),

            UpdatedAt = entry.Sys.UpdatedAt.Value
        };
    }
}