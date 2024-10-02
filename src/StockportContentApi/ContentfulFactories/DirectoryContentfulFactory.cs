namespace StockportContentApi.ContentfulFactories;

public class DirectoryContentfulFactory : IContentfulFactory<ContentfulDirectory, Directory>
{
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subitemFactory;
    private readonly IContentfulFactory<ContentfulExternalLink, ExternalLink> _externalLinkFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulEventBanner, EventBanner> _eventBannerFactory;
    private readonly IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> _directoryEntryFactory;

    public DirectoryContentfulFactory(IContentfulFactory<ContentfulReference, SubItem> subitemFactory,
        IContentfulFactory<ContentfulExternalLink, ExternalLink> externalLinkFactory,
        IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulEventBanner, EventBanner> eventBannerFactory,
        IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry> directoryEntryFactory)
    {
        _subitemFactory = subitemFactory;
        _externalLinkFactory = externalLinkFactory;
        _alertFactory = alertFactory;
        _dateComparer = new DateComparer(timeProvider);
        _callToActionFactory = callToActionFactory;
        _eventBannerFactory = eventBannerFactory;
        _directoryEntryFactory = directoryEntryFactory;
    }

    public Directory ToModel(ContentfulDirectory entry)
    {
        if (entry is null)
            return null;

        IEnumerable<SubItem> subItems = entry.SubDirectories is not null
                ? entry.SubDirectories?
                    .Where(rc => ContentfulHelpers.EntryIsNotALink(rc.Sys))
                    .Select(item => _subitemFactory.ToModel(item))
                : Enumerable.Empty<SubItem>();

        IEnumerable<SubItem> directorySubItems = entry.SubItems?
                            .Where(rc => ContentfulHelpers.EntryIsNotALink(rc.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(rc.SunriseDate, rc.SunsetDate))
                            .Select(item => _subitemFactory.ToModel(item));

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
            Alerts = entry.Alerts?
                        .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Where(alert => !alert.Severity.Equals("Condolence"))
                            .Select(alert => _alertFactory.ToModel(alert)),
            AlertsInline = entry.AlertsInline?
                        .Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Where(alert => !alert.Severity.Equals("Condolence"))
                            .Select(alert => _alertFactory.ToModel(alert)),
            CallToAction = entry.CallToAction is null ? null : _callToActionFactory.ToModel(entry.CallToAction),
            BackgroundImage = entry.BackgroundImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.BackgroundImage.SystemProperties)
                            ? entry.BackgroundImage.File.Url : string.Empty,
            ContentfulId = entry.Sys.Id,
            ColourScheme = entry.ColourScheme,
            SearchBranding = entry.SearchBranding,
            Icon = entry.Icon,
            SubItems = subItems,
            EventBanner = ContentfulHelpers.EntryIsNotALink(entry.EventBanner.Sys)
                            ? _eventBannerFactory.ToModel(entry.EventBanner) : new NullEventBanner(),
            RelatedContent = entry.RelatedContent.Where(rc => ContentfulHelpers.EntryIsNotALink(rc.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(rc.SunriseDate, rc.SunsetDate))
                                .Select(item => _subitemFactory.ToModel(item)).ToList(),
            ExternalLinks = entry.ExternalLinks.Where(el => ContentfulHelpers.EntryIsNotALink(el.Sys))
                                .Select(link => _externalLinkFactory.ToModel(link)).ToList(),
            PinnedEntries = entry.PinnedEntries?.Select(entry => _directoryEntryFactory.ToModel(entry)).ToList()
        };
    }
}