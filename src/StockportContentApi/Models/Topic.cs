namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Topic
{
    public string Title { get; }
    public string Slug { get; }
    public string Teaser { get; }
    public string Summary { get; }
    public string MetaDescription { get; }
    public string Image { get; set; }
    public string Icon { get; }
    public IEnumerable<Alert> Alerts { get; private set; }
    public IEnumerable<Crumb> Breadcrumbs { get; }
    public IEnumerable<SubItem> FeaturedTasks { get; }
    public IEnumerable<SubItem> SubItems { get; }
    public CallToActionBanner CallToAction { get; init; }
    public IEnumerable<SubItem> SecondaryItems { get; }
    public EventBanner EventBanner { get; }
    public string LogoAreaTitle { get; set; }
    public List<TrustedLogo> TrustedLogos { get; set; }

    public string BackgroundImage { get; }
    public TriviaSection TriviaSection { get; init; }
    public Video Video { get; init; }
    public DateTime SunriseDate { get; }
    public DateTime SunsetDate { get; }
    public bool DisplayContactUs { get; }
    public CarouselContent CampaignBanner { get; set; }
    public string EventCategory { get; set; }

    public Topic(string title, string slug, IEnumerable<SubItem> subItems, IEnumerable<SubItem> secondayItems)
    {
        Title = title;
        Slug = slug;
        SubItems = subItems;
        SecondaryItems = secondayItems;
    }

    public Topic(string slug,
                string title,
                string teaser,
                string metaDescription,
                string summary,
                string icon,
                string backgroundImage,
                string image,
                IEnumerable<SubItem> featuredTasks,
                IEnumerable<SubItem> subItems,
                IEnumerable<SubItem> secondayItems,
                IEnumerable<Crumb> breadcrumbs,
                IEnumerable<Alert> alerts,
                DateTime sunriseDate,
                DateTime sunsetDate,
                EventBanner eventBanner,
                CarouselContent campaignBanner,
                string eventCategory,
                CallToActionBanner callToAction,
                List<TrustedLogo> trustedLogos,
                string logoAreaTitle,
                bool displayContactUs = true)
    {
        Slug = slug;
        Title = title;
        Teaser = teaser;
        MetaDescription = metaDescription;
        Summary = summary;
        Icon = icon;
        BackgroundImage = backgroundImage;
        Image = image;
        FeaturedTasks = featuredTasks;
        SubItems = subItems;
        SecondaryItems = secondayItems;
        Breadcrumbs = breadcrumbs;
        Alerts = alerts;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
        EventBanner = eventBanner;
        EventCategory = eventCategory;
        DisplayContactUs = displayContactUs;
        CampaignBanner = campaignBanner;
        CallToAction = callToAction;
        TrustedLogos = trustedLogos;
        LogoAreaTitle = logoAreaTitle;
    }
}

[ExcludeFromCodeCoverage]
public class NullTopic : Topic
{
    public NullTopic() : base(
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        string.Empty,
        new List<SubItem>(),
        new List<SubItem>(),
        new List<SubItem>(),
        new List<Crumb>(),
        new List<Alert>(),
        DateTime.MinValue,
        DateTime.MinValue,
        new NullEventBanner(),
        new CarouselContent(),
        string.Empty,
        null,
        null,
        string.Empty,
        true
        )
    {
    }
}