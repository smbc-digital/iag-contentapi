namespace StockportContentApi.ContentfulFactories.NewsFactories;

public class NewsContentfulFactory(IVideoRepository videoRepository,
                                IContentfulFactory<Asset, Document> documentFactory,
                                IContentfulFactory<ContentfulAlert, Alert> alertFactory,
                                ITimeProvider timeProvider,
                                IContentfulFactory<ContentfulProfile, Profile> profileFactory,
                                IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory,
                                IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> callToActionFactory,
                                IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory) : IContentfulFactory<ContentfulNews, News>
{
    private readonly IVideoRepository _videoRepository = videoRepository;
    private readonly IContentfulFactory<Asset, Document> _documentFactory = documentFactory;
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory = profileFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;
    private readonly IContentfulFactory<ContentfulCallToActionBanner, CallToActionBanner> _callToActionFactory = callToActionFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory = trustedLogoFactory;

    public News ToModel(ContentfulNews entry)
    {
        List<Document> documents = entry.Documents.Where(document => ContentfulHelpers.EntryIsNotALink(document.SystemProperties))
                                    .Select(_documentFactory.ToModel)
                                    .ToList();

        string imageUrl = entry.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.Image.SystemProperties) 
            ? entry.Image.File.Url 
            : string.Empty;

        string heroImageUrl = entry.HeroImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.HeroImage.SystemProperties) 
            ? entry.HeroImage.File.Url 
            : string.Empty;

        string teaserImageUrl = entry.TeaserImage?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(entry.TeaserImage.SystemProperties) 
            ? entry.TeaserImage.File.Url 
            : string.Empty;

        IEnumerable<Alert> alerts = entry.Alerts.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys) 
                                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(section.SunriseDate, section.SunsetDate))
                                        .Where(alert => !alert.Severity.Equals("Condolence"))
                                        .Select(_alertFactory.ToModel);

        DateTime? updatedAt = entry.Sys.UpdatedAt is not null
            ? entry.Sys.UpdatedAt
            : entry.SunriseDate;

        List<Profile> profiles = entry.Profiles.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                    .Select(_profileFactory.ToModel).ToList();

        DateTimeOffset sunriseWithOffset = DateTimeOffset.Parse(entry.SunriseDate.ToString("o"));
        DateTime utcDateTime = sunriseWithOffset.UtcDateTime;

        DateTime sunrise = sunriseWithOffset.Offset.Hours > 0
            ? utcDateTime.AddHours(1)
            : utcDateTime;

        bool hasOffset = sunriseWithOffset.Offset.Hours > 0;
        
        return new News(entry.Title,
                        entry.Slug,
                        entry.Teaser,
                        entry.Purpose,
                        imageUrl,
                        heroImageUrl,
                        ImageConverter.SetThumbnailWithoutHeight(imageUrl, teaserImageUrl),
                        entry.HeroImageCaption,
                        entry.Body,
                        sunrise,
                        entry.SunsetDate,
                        hasOffset,
                        sunriseWithOffset,
                        entry.Sys.UpdatedAt.Value,
                        new List<Crumb> { new("News", string.Empty, "news") },
                        alerts.ToList(),
                        entry.Tags,
                        documents,
                        entry.Categories,
                        profiles,
                        entry.InlineQuotes.Select(_inlineQuoteContentfulFactory.ToModel).ToList(),
                        _callToActionFactory.ToModel(entry.CallToAction),
                        entry.LogoAreaTitle,
                        entry.TrustedLogos is not null
                            ? entry.TrustedLogos.Where(trustedLogo => trustedLogo is not null)
                                                .Select(_trustedLogoFactory.ToModel).ToList()
                            : new(),
                        entry.FeaturedLogo is not null
                            ? _trustedLogoFactory.ToModel(entry.FeaturedLogo)
                            : null,
                        entry.EventsByTagOrCategory);
    }
}