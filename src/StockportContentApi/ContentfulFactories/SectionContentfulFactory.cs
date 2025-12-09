namespace StockportContentApi.ContentfulFactories;

public class SectionContentfulFactory(IContentfulFactory<ContentfulProfile, Profile> profileFactory,
                                    IContentfulFactory<Asset, Document> documentFactory, IVideoRepository videoRepository,
                                    ITimeProvider timeProvider, IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                    IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory,
                                    IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory) : IContentfulFactory<ContentfulSection, Section>
{
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<Asset, Document> _documentFactory = documentFactory;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory = profileFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory = trustedLogoFactory;
    private readonly IVideoRepository _videoRepository = videoRepository;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;

    public Section ToModel(ContentfulSection entry)
    {
        List<Profile> profiles = entry.Profiles.Where(profile => ContentfulHelpers.EntryIsNotALink(profile.Sys))
                                    .Select(_profileFactory.ToModel).ToList();

        List<Document> documents = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                    .Select(_documentFactory.ToModel).ToList();

        string body = _videoRepository.Process(entry.Body);

        IEnumerable<Alert> alertsInline = entry.AlertsInline.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                            .Where(alert => !alert.Severity.Equals("Condolence"))
                                            .Select(_alertFactory.ToModel);

        List<TrustedLogo> trustedLogos = entry.TrustedLogos is not null
            ? entry.TrustedLogos.Where(trustedLogo => trustedLogo is not null).Select(_trustedLogoFactory.ToModel).ToList()
            : new();

        bool hasLastEditorialUpdate = entry.LastEditorialUpdate is not null && !entry.LastEditorialUpdate.Equals(DateTime.MinValue);
        bool hasTaggedPublishedDate = entry.TaggedPublishedDate is not null && !entry.TaggedPublishedDate.Equals(DateTime.MinValue);

        DateTime updatedAt = hasLastEditorialUpdate && hasTaggedPublishedDate
            ? TruncateToMinutes(entry.Sys.UpdatedAt.Value) > TruncateToMinutes(entry.TaggedPublishedDate.Value)
                ? entry.Sys.UpdatedAt.Value
                : entry.LastEditorialUpdate.Value
            : entry.Sys.UpdatedAt.Value;

        return new(entry.Title,
                entry.Slug,
                entry.MetaDescription,
                body,
                profiles,
                documents,
                entry.LogoAreaTitle,
                trustedLogos,
                updatedAt,
                alertsInline,
                entry.InlineQuotes.Select(_inlineQuoteContentfulFactory.ToModel).ToList());
    }

    
    private static DateTime TruncateToMinutes(DateTime dateTime) =>
        new(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
}