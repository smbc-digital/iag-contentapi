namespace StockportContentApi.ContentfulFactories;

public class PublicationPageContentfulFactory(IContentfulFactory<ContentfulPublicationSection, PublicationSection> publicationSectionFactory,
    IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory) : IContentfulFactory<ContentfulPublicationPage, PublicationPage>
{
    private readonly IContentfulFactory<ContentfulPublicationSection, PublicationSection> _publicationSectionFactory = publicationSectionFactory;
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
            PublicationSections = entry.PublicationSections is not null
                ? entry.PublicationSections.Where(page => page is not null).Select(_publicationSectionFactory.ToModel).ToList()
                : new(),
            Body = entry.Body,
            LogoAreaTitle = entry.LogoAreaTitle,
            TrustedLogos = entry.TrustedLogos is not null
                ? entry.TrustedLogos.Where(trustedLogo => trustedLogo is not null).Select(_trustedLogoFactory.ToModel).ToList()
                : new()
        };
    }
}