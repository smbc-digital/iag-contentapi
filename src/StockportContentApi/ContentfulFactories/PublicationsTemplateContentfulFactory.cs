namespace StockportContentApi.ContentfulFactories;

public class PublicationsTemplateContentfulFactory(IContentfulFactory<ContentfulPublicationPage, PublicationPage> publicationPageFactory,
                                    ITimeProvider timeProvider) : IContentfulFactory<ContentfulPublicationsTemplate, PublicationsTemplate>
{
    private readonly IContentfulFactory<ContentfulPublicationPage, PublicationPage> _publicationPageFactory = publicationPageFactory;
    private readonly DateComparer _dateComparer = new(timeProvider);

    public PublicationsTemplate ToModel(ContentfulPublicationsTemplate entry)
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

        return new PublicationsTemplate
        {
            Title = entry.Title,
            Slug = entry.Slug,
            MetaDescription = entry.MetaDescription,
            HeroImage = heroImage,
            Subtitle = entry.Subtitle,
            DisplayReviewDate = entry.DisplayReviewDate,
            PublicationPages = entry.PublicationPages.Where(page => ContentfulHelpers.EntryIsNotALink(page.Sys)
                            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(page.SunriseDate, page.SunsetDate))
                        .Select(_publicationPageFactory.ToModel).ToList(),
            ColourScheme = entry.ColourScheme,
            Body = entry.Body
        };
    }
}