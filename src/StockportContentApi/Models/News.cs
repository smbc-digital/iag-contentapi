namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class News(string title,
                string slug,
                string teaser,
                string image,
                string heroImage,
                string thumbnailImage,
                string heroImageCaption,
                string body,
                DateTime sunriseDate,
                DateTime sunsetDate,
                DateTime updatedAt,
                List<Alert> alerts,
                List<string> tags,
                List<Document> documents,
                List<string> categories,
                IEnumerable<Profile> profiles,
                List<InlineQuote> inlineQuotes,
                CallToActionBanner callToAction,
                string logoAreaTitle,
                List<TrustedLogo> trustedLogos,
                TrustedLogo featuredLogo,
                string eventsByTagOrCategory)
{
    public string Title { get; } = title;
    public string Slug { get; } = slug;
    public string Teaser { get; } = teaser;
    public string Image { get; } = image;
    public string HeroImage { get; } = heroImage;
    public string ThumbnailImage { get; } = thumbnailImage;
    public string HeroImageCaption { get; } = heroImageCaption;
    public string Body { get; } = body;
    public DateTime SunriseDate { get; } = sunriseDate;
    public string PublishingDate { get; set; }
    public DateTime SunsetDate { get; } = sunsetDate;
    public DateTime UpdatedAt { get; } = updatedAt;
    public List<string> Tags { get; set; } = tags;
    public List<Alert> Alerts { get; } = alerts;
    public List<Document> Documents { get; } = documents;
    public List<string> Categories { get; } = categories;
    public IEnumerable<Profile> Profiles { get; } = profiles;
    public List<InlineQuote> InlineQuotes { get; set; } = inlineQuotes;
    public CallToActionBanner CallToAction { get; init; } = callToAction;
    public string LogoAreaTitle { get; set; } = logoAreaTitle;
    public List<TrustedLogo> TrustedLogos { get; set; } = trustedLogos;
    public TrustedLogo FeaturedLogo { get; set; } = featuredLogo;
    public string EventsByTagOrCategory { get; set;} = eventsByTagOrCategory;
    public List<Event> Events { get; set; }
}