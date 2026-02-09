namespace StockportContentApi.ContentfulFactories;

public class PublicationSectionContentfulFactory(ITimeProvider timeProvider,
    IContentfulFactory<ContentfulAlert, Alert> alertFactory,
    IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory,
    IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory) : IContentfulFactory<ContentfulPublicationSection, PublicationSection>
{
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory = trustedLogoFactory;

    public PublicationSection ToModel(ContentfulPublicationSection entry)
    {
        if (entry is null)
            return null;

        return new PublicationSection
        {
            Title = entry.Title,
            Slug = entry.Slug,
            MetaDescription = entry.MetaDescription,
            Body = entry.Body,
            InlineAlerts = entry.InlineAlerts.Where(alert => ContentfulHelpers.EntryIsNotALink(alert.Sys)
                                                && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(alert.SunriseDate, alert.SunsetDate))
                                            .Where(alert => !alert.Severity.Equals("Condolence"))
                                            .Select(_alertFactory.ToModel),
            InlineQuotes = entry.InlineQuotes.Select(_inlineQuoteContentfulFactory.ToModel).ToList(),
            LogoAreaTitle = entry.LogoAreaTitle,
            TrustedLogos = entry.TrustedLogos is not null
                ? entry.TrustedLogos.Where(trustedLogo => trustedLogo is not null).Select(_trustedLogoFactory.ToModel).ToList()
                : new()
        };
    }
}