namespace StockportContentApi.ContentfulFactories;

public class PublicationSectionContentfulFactory(IContentfulFactory<ContentfulTrustedLogo, TrustedLogo> trustedLogoFactory)
    : IContentfulFactory<ContentfulPublicationSection, PublicationSection>
{
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
            LogoAreaTitle = entry.LogoAreaTitle,
            TrustedLogos = entry.TrustedLogos is not null
                ? entry.TrustedLogos.Where(trustedLogo => trustedLogo is not null).Select(_trustedLogoFactory.ToModel).ToList()
                : new()
        };
    }
}