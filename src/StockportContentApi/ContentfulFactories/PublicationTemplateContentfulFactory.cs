namespace StockportContentApi.ContentfulFactories;

public class PublicationTemplateContentfulFactory(IContentfulFactory<ContentfulPublicationPage, PublicationPage> publicationPageFactory,
    IContentfulFactory<ContentfulReference, Crumb> crumbFactory,
    ITimeProvider timeProvider,
    IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory) : IContentfulFactory<ContentfulPublicationTemplate, PublicationTemplate>
{
    private readonly IContentfulFactory<ContentfulPublicationPage, PublicationPage> _publicationPageFactory = publicationPageFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory = crumbFactory;
    private readonly IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> _trustedLogoFactory = trustedLogoFactory;

    public PublicationTemplate ToModel(ContentfulPublicationTemplate entry)
    {
        if (entry is null)
            return null;

        MediaAsset headerImage = new();

        if (entry.HeaderImage is not null && entry.HeaderImage.File is not null)
            headerImage = new()
            {
                Url = entry.HeaderImage.File.Url,
                Description = entry.HeaderImage.Description
            };

        return new PublicationTemplate
        {
            Title = entry.Title,
            Slug = entry.Slug,
            MetaDescription = entry.MetaDescription,
            Breadcrumbs = entry.Breadcrumbs
                                    .Where(crumb => ContentfulHelpers.EntryIsNotALink(crumb.Sys))
                                    .Select(_crumbFactory.ToModel).ToList(),
            Subtitle = entry.Subtitle,
            Summary = entry.Summary,
            HeaderImage = headerImage,
            DatePublished = entry.DatePublished,
            ReviewDate = entry.ReviewDate,
            PublicationTheme = entry.PublicationTheme,
            PublicationPages = entry.PublicationPages.Where(page => ContentfulHelpers.EntryIsNotALink(page.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(page.SunriseDate, page.SunsetDate))
                        .Select(_publicationPageFactory.ToModel).ToList(),
            LogoAreaTitle =  entry.LogoAreaTitle,
            TrustedLogos = entry.TrustedLogos is not null
                ? entry.TrustedLogos.Where(trustedLogo => trustedLogo is not null).Select(_trustedLogoFactory.ToModel).ToList()
                : new()
        };
    }
}