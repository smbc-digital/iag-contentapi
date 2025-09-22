namespace StockportContentApi.ContentfulFactories;

public class DirectoryContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
                                        IContentfulFactory<ContentfulExternalLink, ExternalLink> externalLinkFactory,
                                        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
                                        ITimeProvider timeProvider,
                                        IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory,
                                        IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> directoryEntryFactory) : IContentfulFactory<ContentfulDirectory, Directory>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory = subitemFactory;
    private readonly IContentfulFactory<ContentfulExternalLink, ExternalLink> _externalLinkFactory = externalLinkFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory = callToActionFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory = eventBannerFactory;
    private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory = directoryEntryFactory;

    public Directory ToModel(ContentfulDirectory entry)
    {
        if (entry is null)
            return null;

        IEnumerable<SubItem> subItems = entry.SubDirectories is not null
                                            ? entry.SubDirectories?.Where(subDirectory => ContentfulHelpers.EntryIsNotALink(subDirectory.Sys))
                                                                .Select(_subitemFactory.ToModel)
                                            : Enumerable.Empty<SubItem>();

        IEnumerable<SubItem> directorySubItems = entry.SubItems?.Where(subDirectory => ContentfulHelpers.EntryIsNotALink(subDirectory.Sys)
                                                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(subDirectory.SunriseDate, subDirectory.SunsetDate))
                                                                .Select(_subitemFactory.ToModel);

        subItems = directorySubItems is not null
            ? subItems.Concat(directorySubItems)
            : subItems;

        return new()
        {
            Slug = entry.Slug,
            Title = entry.Title,
            Body = entry.Body,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            Alerts = entry.Alerts?.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys) 
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                        .Where(alert => !alert.Severity.Equals("Condolence"))
                        .Select(_alertFactory.ToModel),
            
            AlertsInline = entry.AlertsInline?.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Where(alert => !alert.Severity.Equals("Condolence"))
                            .Select(_alertFactory.ToModel),
            
            CallToAction = entry.CallToAction is null 
                ? null 
                : _callToActionFactory.ToModel(entry.CallToAction),
            
            BackgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                ? entry.BackgroundImage.File.Url 
                : string.Empty,
            
            ContentfulId = entry.Sys.Id,
            ColourScheme = entry.ColourScheme,
            SearchBranding = entry.SearchBranding,
            Icon = entry.Icon,
            SubItems = subItems,
            EventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.Sys)
                ? _eventBannerFactory.ToModel(entry.EventBanner)
                : new NullEventBanner(),
            
            RelatedContent = entry.RelatedContent.Where(relatedContent => ContentfulHelpers.EntryIsNotALink(relatedContent.Sys)
                                    && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(relatedContent.SunriseDate, relatedContent.SunsetDate))
                                .Select(_subitemFactory.ToModel).ToList(),
            
            ExternalLinks = entry.ExternalLinks.Where(externalLink => ContentfulHelpers.EntryIsNotALink(externalLink.Sys))
                                .Select(_externalLinkFactory.ToModel).ToList(),
            
            PinnedEntries = entry.PinnedEntries?.Select(_directoryEntryFactory.ToModel).ToList()
        };
    }
}