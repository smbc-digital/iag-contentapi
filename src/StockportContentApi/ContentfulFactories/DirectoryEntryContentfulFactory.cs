namespace StockportContentApi.ContentfulFactories;
public class DirectoryEntryContentfulFactory(IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                            IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory,
                                            ITimeProvider timeProvider) : IContentfulFactory<ContentfulDirectoryEntry, DirectoryEntry>
{
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory = trustedLogoFactory;
    private readonly DateComparer _dateComparer = new DateComparer(timeProvider);

    public DirectoryEntry ToModel(ContentfulDirectoryEntry entry)
    {
        if (entry is null)
            return null;

        DirectoryEntry directoryEntry = new()
        {
            Slug = entry.Slug,
            Provider = entry.Provider,
            Name = entry.Name,
            Description = entry.Description,
            Teaser = entry.Teaser,
            MetaDescription = entry.MetaDescription,
            MapPosition = entry.MapPosition,
            Tags = entry.Tags,
            PhoneNumber = entry.PhoneNumber,
            Email = entry.Email,
            Website = entry.Website,
            Twitter = entry.Twitter,
            Facebook = entry.Facebook,
            Youtube = entry.Youtube,
            Instagram = entry.Instagram,
            LinkedIn = entry.LinkedIn,
            Address = entry.Address,
            
            Image = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) 
                ? entry.Image.File.Url 
                : string.Empty,
            
            Directories = entry.Directories?.Select(contentfulDirectory => new MinimalDirectory(contentfulDirectory.Slug, contentfulDirectory.Title)),
            
            Alerts = entry.Alerts?.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                        .Where(alert => !alert.Severity.Equals("Condolence"))
                        .Select(_alertFactory.ToModel),

            AlertsInline = entry.AlertsInline?.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                            .Where(alert => !alert.Severity.Equals("Condolence"))
                            .Select(_alertFactory.ToModel),
            
            Themes = entry.Filters?.Select(filter => filter.Theme)
                        .Distinct()
                        .Select(theme => new FilterTheme()
                        {
                            Title = theme,
                            Filters = entry.Filters
                                .Where(filter => !string.IsNullOrEmpty(filter.Theme)
                                    && filter.Theme.Equals(theme))
                                .Select(filter => new Filter(filter))
                        }),
            
            TrustedLogos = entry.TrustedLogos?.Select(_trustedLogoFactory.ToModel)
        };

        return directoryEntry;
    }
}