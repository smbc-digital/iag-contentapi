namespace StockportContentApi.ContentfulFactories;

public class SectionContentfulFactory(IContentfulFactory<ContentfulProfile, Profile> profileFactory,
                                    IContentfulFactory<Asset, Document> documentFactory,
                                    IVideoRepository videoRepository,
                                    ITimeProvider timeProvider,
                                    IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                    IContentfulFactory<ContentfulTrustedLogos, TrustedLogos> sectionBrandingFactory) : IContentfulFactory<ContentfulSection, Section>
{
    private readonly DateComparer _dateComparer = new DateComparer(timeProvider);
    private readonly IContentfulFactory<Asset, Document> _documentFactory = documentFactory;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory = profileFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogos, TrustedLogos> _sectionBrandingFactory = sectionBrandingFactory;
    private readonly IVideoRepository _videoRepository = videoRepository;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;

    public Section ToModel(ContentfulSection entry)
    {
        List<Profile> profiles = entry.Profiles.Where(profile => ContentfulHelpers.EntryIsNotALink(profile.Sys))
                                    .Select(_profileFactory.ToModel).ToList();

        List<Document> documents = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                    .Select(_documentFactory.ToModel).ToList();

        string body = _videoRepository.Process(entry.Body);

        IEnumerable<Alert> alertsInline = entry.AlertsInline.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys)
                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                            .Where(alert => !alert.Severity.Equals("Condolence"))
                                            .Select(_alertFactory.ToModel);

        List<TrustedLogos> sectionBranding = entry.SectionBranding is not null
            ? entry.SectionBranding.Where(branding => branding is not null).Select(_sectionBrandingFactory.ToModel).ToList()
            : new();

        DateTime updatedAt = entry.Sys.UpdatedAt.Value;

        return new(entry.Title,
                entry.Slug,
                entry.MetaDescription,
                body,
                profiles,
                documents,
                entry.LogoAreaTitle,
                sectionBranding,
                entry.SunriseDate,
                entry.SunsetDate,
                updatedAt,
                alertsInline);
    }
}