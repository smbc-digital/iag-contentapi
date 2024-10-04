namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Topic
{
    public string Slug { get; }
    public string Name { get; }
    public string Teaser { get; }
    public string MetaDescription { get; }
    public string Summary { get; }
    public string Icon { get; }
    public string BackgroundImage { get; }
    public TriviaSection TriviaSection { get; init; }
    public string Image { get; set; }
    public Video Video { get; init; }
    public IEnumerable<SubItem> FeaturedTasks { get; }
    public IEnumerable<SubItem> SubItems { get; }
    public IEnumerable<SubItem> SecondaryItems { get; }
    public IEnumerable<Crumb> Breadcrumbs { get; }
    public IEnumerable<Alert> Alerts { get; private set; }
    public DateTime SunriseDate { get; }
    public DateTime SunsetDate { get; }
    public bool EmailAlerts { get; }
    public string EmailAlertsTopicId { get; }
    public EventBanner EventBanner { get; }
    public bool DisplayContactUs { get; }
    public CarouselContent CampaignBanner { get; set; }
    public CallToActionBanner CallToAction { get; init; }
    public string EventCategory { get; set; }
    public List<GroupBranding> TopicBranding { get; set; }
    public string LogoAreaTitle { get; set; }

    public Topic(string title, string slug, IEnumerable<SubItem> subItems, IEnumerable<SubItem> secondayItems)
    {
        Name = title;
        Slug = slug;
        SubItems = subItems;
        SecondaryItems = secondayItems;
    }

    public Topic(string slug, string name, string teaser, string metaDescription, string summary, string icon, string backgroundImage,
        string image, IEnumerable<SubItem> featuredTasks, IEnumerable<SubItem> subItems, IEnumerable<SubItem> secondayItems, IEnumerable<Crumb> breadcrumbs,
        IEnumerable<Alert> alerts, DateTime sunriseDate, DateTime sunsetDate, bool emailAlerts,
        string emailAlertsTopicId, EventBanner eventBanner, CarouselContent campaignBanner, string eventCategory,
        CallToActionBanner callToAction, List<GroupBranding> topicBranding, string logoAreaTitle,
        bool displayContactUs = true)
    {
        Slug = slug;
        Name = name;
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
        EmailAlerts = emailAlerts;
        EmailAlertsTopicId = emailAlertsTopicId;
        EventBanner = eventBanner;
        EventCategory = eventCategory;
        DisplayContactUs = displayContactUs;
        CampaignBanner = campaignBanner;
        CallToAction = callToAction;
        TopicBranding = topicBranding;
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
        false,
        string.Empty,
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