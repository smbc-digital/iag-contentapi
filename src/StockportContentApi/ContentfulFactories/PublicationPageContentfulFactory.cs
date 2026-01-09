namespace StockportContentApi.ContentfulFactories;

public class PublicationPageContentfulFactory(IContentfulFactory<ContentfulPublicationSection, PublicationSection> publicationSectionFactory,
                                    ITimeProvider timeProvider) : IContentfulFactory<ContentfulPublicationPage, PublicationPage>
{
    private readonly IContentfulFactory<ContentfulPublicationSection, PublicationSection> _publicationSectionFactory = publicationSectionFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);

    public PublicationPage ToModel(ContentfulPublicationPage entry)
    {
        if (entry is null)
            return null;

        MediaAsset heroImage = new();

        if (entry.HeroImage is not null && entry.HeroImage.File is not null)
            heroImage = new()
            {
                Url = entry.HeroImage.File.Url,
                Description = entry.HeroImage.Description
            };

        return new PublicationPage
        {
            Title = entry.Title,
            Slug = entry.Slug,
            MetaDescription = entry.MetaDescription,
            HeroImage = heroImage,
            PublicationSections = entry.PublicationSections.Where(page => ContentfulHelpers.EntryIsNotALink(page.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(page.SunriseDate, page.SunsetDate))
                        .Select(_publicationSectionFactory.ToModel).ToList(),
            Body = entry.Body
        };
    }
}