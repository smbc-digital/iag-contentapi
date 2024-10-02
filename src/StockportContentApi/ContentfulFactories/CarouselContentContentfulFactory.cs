namespace StockportContentApi.ContentfulFactories;

public class CarouselContentContentfulFactory : IContentfulFactory<ContentfulCarouselContent, CarouselContent>
{
    public CarouselContent ToModel(ContentfulCarouselContent carousel)
    {
        string title = carousel.Title ?? string.Empty;
        string slug = carousel.Slug ?? string.Empty;
        string teaser = carousel.Teaser ?? string.Empty;
        string image = carousel.Image?.SystemProperties is not null && ContentfulHelpers.EntryIsNotALink(carousel.Image.SystemProperties)
            ? carousel.Image.File.Url
            : string.Empty;
        string url = carousel.Url ?? string.Empty;
        DateTime sunriseDate = DateComparer.DateFieldToDate(carousel.SunriseDate);
        DateTime sunsetDate = DateComparer.DateFieldToDate(carousel.SunsetDate);

        return new CarouselContent(title, slug, teaser, image, sunriseDate, sunsetDate, url);
    }
}