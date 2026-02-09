namespace StockportContentApi.ContentfulFactories;

public class PublicationPageContentfulFactory(IContentfulFactory<ContentfulPublicationSection, PublicationSection> publicationSectionFactory,
    ITimeProvider timeProvider,
    IContentfulFactory<ContentfulAlert, Alert> alertFactory,
    IContentfulFactory<ContentfulInlineQuote, InlineQuote> inlineQuoteContentfulFactory,
    IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory) : IContentfulFactory<ContentfulPublicationPage, PublicationPage>
{
    private readonly IContentfulFactory<ContentfulPublicationSection, PublicationSection> _publicationSectionFactory = publicationSectionFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulAlert, Alert> _alertFactory = alertFactory;
    private readonly IContentfulFactory<ContentfulInlineQuote, InlineQuote> _inlineQuoteContentfulFactory = inlineQuoteContentfulFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory = trustedLogoFactory;

    public PublicationPage ToModel(ContentfulPublicationPage entry)
    {
        if (entry is null)
            return null;

        return new PublicationPage
        {
            Title = entry.Title,
            Slug = entry.Slug,
            MetaDescription = entry.MetaDescription,
            PublicationSections = entry.PublicationSections.Where(page => ContentfulHelpers.EntryIsNotALink(page.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(page.SunriseDate, page.SunsetDate))
                        .Select(_publicationSectionFactory.ToModel).ToList(),
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