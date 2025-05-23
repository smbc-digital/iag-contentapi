namespace StockportContentApi.ContentfulFactories.EventFactories;

public class EventContentfulFactory(IContentfulFactory<Asset, Document> documentFactory,
                            IContentfulFactory<ContentfulGroup, Group> groupFactory,
                            IContentfulFactory<ContentfulEventCategory, EventCategory> eventCategoryFactory,
                            IContentfulFactory<ContentfulGroupBranding, GroupBranding> brandingFactory,
                            IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                            IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionContentfulFactory,
                            ITimeProvider timeProvider) : IContentfulFactory<ContentfulEvent, Event>
{
    private readonly IContentfulFactory<Asset, Document> _documentFactory = documentFactory;
    private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory = groupFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulEventCategory, EventCategory> _eventCategoryFactory = eventCategoryFactory;
    private readonly IContentfulFactory<ContentfulGroupBranding, GroupBranding> _brandingFactory = brandingFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionContentfulFactory = callToActionContentfulFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);

    public Event ToModel(ContentfulEvent entry)
    {
        List<Document> eventDocuments = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                            .Select(_documentFactory.ToModel).ToList();

        string imageUrl = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) 
            ? entry.Image?.File?.Url 
            : string.Empty;


        string thumbnailImageUrl = entry.ThumbnailImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.ThumbnailImage.SystemProperties) 
            ? entry.ThumbnailImage?.File?.Url 
            : string.Empty;

        Group group = _groupFactory.ToModel(entry.Group);

        IEnumerable<EventCategory> categories = entry.EventCategories.Select(_eventCategoryFactory.ToModel);

        List<Alert> alerts = entry.Alerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys) 
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                .Where(alert => !alert.Severity.Equals("Condolence"))
                                .Select(_alertFactory.ToModel).ToList();
        
        List<GroupBranding> eventBranding = entry.EventBranding?.Select(_brandingFactory.ToModel).ToList();

        return new Event(entry.Title,
                        entry.Slug,
                        entry.Teaser,
                        imageUrl,
                        entry.Description,
                        entry.Fee,
                        entry.Location,
                        entry.SubmittedBy,
                        entry.EventDate,
                        entry.StartTime,
                        entry.EndTime,
                        entry.Occurences,
                        entry.Frequency,
                        new List<Crumb> { new("Events", string.Empty, "events") },
                        ImageConverter.SetThumbnailWithoutHeight(imageUrl, thumbnailImageUrl),
                        eventDocuments,
                        entry.MapPosition,
                        entry.Featured,
                        entry.BookingInformation,
                        entry.Sys.UpdatedAt,
                        entry.Tags,
                        group,
                        alerts,
                        categories.ToList(),
                        entry.Free,
                        entry.Paid,
                        entry.LogoAreaTitle,
                        eventBranding,
                        entry.PhoneNumber,
                        entry.Email,
                        entry.Website,
                        entry.Facebook,
                        entry.Instagram,
                        entry.LinkedIn,
                        entry.MetaDescription,
                        entry.Duration,
                        entry.Languages,
                        entry.CallToActionBanners.Select(_callToActionContentfulFactory.ToModel).ToList());
    }
}