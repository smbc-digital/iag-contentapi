namespace StockportContentApi.ContentfulFactories;

public class DocumentPageContentfulFactory(IContentfulFactory<Asset, Document> documentFactory,
                                        IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
                                        IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
                                        ITimeProvider timeProvider) : IContentfulFactory<ContentfulDocumentPage, DocumentPage>
{
    private readonly IContentfulFactory<Asset, Document> _documentFactory = documentFactory;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory = subitemFactory;
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);

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
                            .Select(_documentFactory.ToModel).ToList(),
            
            AwsDocuments = entry.AwsDocuments,
            RequestAnAccessibleFormatContactInformation = entry.RequestAnAccessibleFormatContactInformation,
            FurtherInformation = entry.FurtherInformation,

            RelatedDocuments = entry.RelatedDocuments.Where(subItem => ContentfulHelpers.EntryIsNotALink(subItem.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subItem.SunriseDate, subItem.SunsetDate))
                                .Select(_subitemFactory.ToModel).ToList(),

            DatePublished = entry.DatePublished,
            LastUpdated = entry.LastUpdated,

            Breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                            .Select(_crumbFactory.ToModel).ToList(),

            UpdatedAt = entry.Sys.UpdatedAt.Value
        };
    }
}