namespace StockportContentApi.ContentfulFactories;

public class SectionContentfulFactory : IContentfulFactory<ContentfulSection, Section>
{
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<Asset, Document> _documentFactory;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;
    private readonly IContentfulFactory<ContentfulGroupBranding, GroupBranding> _sectionBrandingFactory;
    private readonly IVideoRepository _videoRepository;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory;

    public SectionContentfulFactory(IContentfulFactory<ContentfulProfile, Profile> profileFactory,
        IContentfulFactory<Asset, Document> documentFactory, IVideoRepository videoRepository,
        ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory,
        IContentfulFactory<ContentfulGroupBranding, GroupBranding> sectionBrandingFactory)
    {
        _profileFactory = profileFactory;
        _documentFactory = documentFactory;
        _videoRepository = videoRepository;
        _dateComparer = new DateComparer(timeProvider);
        _alertFactory = alertFactory;
        _sectionBrandingFactory = sectionBrandingFactory;
    }

    public Section ToModel(ContentfulSection entry)
    {
        List<Profile> profiles = entry.Profiles.Where(profile => ContentfulHelpers.EntryIsNotALink(profile.Sys))
                                    .Select(profile => _profileFactory.ToModel(profile)).ToList();

        List<Document> documents = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                    .Select(document => _documentFactory.ToModel(document)).ToList();

        string body = _videoRepository.Process(entry.Body);

        IEnumerable<Alert> alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                            .Where(alert => !alert.Severity.Equals("Condolence"))
                                            .Select(alertInline => _alertFactory.ToModel(alertInline));

        List<GroupBranding> sectionBranding = entry.SectionBranding is not null
            ? entry.SectionBranding.Where(_ => _ is not null)
                .Select(branding => _sectionBrandingFactory.ToModel(branding)).ToList()
            : new();

        DateTime updatedAt = entry.Sys.UpdatedAt.Value;

        return new(entry.Title, entry.Slug, entry.MetaDescription,
            body, profiles, documents, entry.LogoAreaTitle, sectionBranding,
            entry.SunriseDate, entry.SunsetDate, updatedAt, alertsInline);
    }
}