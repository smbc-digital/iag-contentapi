namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class News(string title,
                string slug,
                string teaser,
                string purpose,
                string image,
                string heroImage,
                string thumbnailImage,
                string heroImageCaption,
                string body,
                DateTime sunriseDate,
                DateTime sunsetDate,
                DateTime updatedAt,
                List<Crumb> breadcrumbs,
                List<Alert> alerts,
                List<string> tags,
                List<Document> documents,
                List<string> categories,
                IEnumerable<Profile> profiles,
                List<InlineQuote> inlineQuotes,
                CallToActionBanner callToAction,
                string logoAreaTitle,
                List<GroupBranding> newsBranding,
                GroupBranding featuredLogo,
                string eventsByTagOrCategory)
{
    public string Title { get; } = title;
    public string Slug { get; } = slug;
    public string Teaser { get; } = teaser;
    public string Purpose { get; set; } = purpose;
    public string Image { get; } = image;
    public string HeroImage { get; } = heroImage;
    public string ThumbnailImage { get; } = thumbnailImage;
    public string HeroImageCaption { get; } = heroImageCaption;
    public string Body { get; } = body;
    public DateTime SunriseDate { get; } = sunriseDate;
    public DateTime SunsetDate { get; } = sunsetDate;
    public DateTime UpdatedAt { get; } = updatedAt;
    public List<Crumb> Breadcrumbs { get; } = breadcrumbs;
    public List<string> Tags { get; set; } = tags;
    public List<Alert> Alerts { get; } = alerts;
    public List<Document> Documents { get; } = documents;
    public List<string> Categories { get; } = categories;
    public IEnumerable<Profile> Profiles { get; } = profiles;
    public List<InlineQuote> InlineQuotes { get; set; } = inlineQuotes;
    public CallToActionBanner CallToAction { get; init; } = callToAction;
    public string LogoAreaTitle { get; set; } = logoAreaTitle;
    public List<GroupBranding> NewsBranding { get; set; } = newsBranding;
    public GroupBranding FeaturedLogo { get; set; } = featuredLogo;
    public string EventsByTagOrCategory { get; set;} = eventsByTagOrCategory;
    public List<Event> Events { get; set; }
}