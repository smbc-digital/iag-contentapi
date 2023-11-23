namespace StockportContentApi.Model;

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
    public IEnumerable<SubItem> SubItems { get; }
    public IEnumerable<SubItem> SecondaryItems { get; }
    public IEnumerable<SubItem> TertiaryItems { get; }
    public IEnumerable<Crumb> Breadcrumbs { get; }
    public IEnumerable<Alert> Alerts { get; private set; }
    public DateTime SunriseDate { get; }
    public DateTime SunsetDate { get; }
    public bool EmailAlerts { get; }
    public string EmailAlertsTopicId { get; }
    public EventBanner EventBanner { get; }
    public string ExpandingLinkTitle { get; }
    public IEnumerable<ExpandingLinkBox> ExpandingLinkBoxes { get; }
    public string PrimaryItemTitle { get; set; }
    public bool DisplayContactUs { get; }
    public CarouselContent CampaignBanner { get; set; }
    public CallToActionBanner CallToAction { get; init; }
    public string EventCategory { get; set; }
    public List<GroupBranding> TopicBranding { get; set; }
    public string LogoAreaTitle { get; set; }

    public Topic(string title, string slug, IEnumerable<SubItem> subItems, IEnumerable<SubItem> secondayItems,
        IEnumerable<SubItem> tertiaryItems)
    {
        Name = title;
        Slug = slug;
        SubItems = subItems;
        SecondaryItems = secondayItems;
        TertiaryItems = tertiaryItems;
    }

    public Topic(string slug, string name, string teaser, string metaDescription, string summary, string icon, string backgroundImage,
        string image, IEnumerable<SubItem> subItems, IEnumerable<SubItem> secondayItems, IEnumerable<SubItem> tertiaryItems,
        IEnumerable<Crumb> breadcrumbs, IEnumerable<Alert> alerts, DateTime sunriseDate, DateTime sunsetDate, bool emailAlerts,
        string emailAlertsTopicId, EventBanner eventBanner, string expandingLinkTitle, CarouselContent campaignBanner, string eventCategory,
        CallToActionBanner callToAction, List<GroupBranding> topicBranding,  string logoAreaTitle, IEnumerable<ExpandingLinkBox> expandingLinkBoxes = null, string primaryItemTitle = null,
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
        SubItems = subItems;
        SecondaryItems = secondayItems;
        TertiaryItems = tertiaryItems;
        Breadcrumbs = breadcrumbs;
        Alerts = alerts;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
        EmailAlerts = emailAlerts;
        EmailAlertsTopicId = emailAlertsTopicId;
        EventBanner = eventBanner;
        EventCategory = eventCategory;
        ExpandingLinkTitle = expandingLinkTitle;
        ExpandingLinkBoxes = expandingLinkBoxes;
        PrimaryItemTitle = primaryItemTitle;
        DisplayContactUs = displayContactUs;
        CampaignBanner = campaignBanner;
        CallToAction = callToAction;
        TopicBranding = topicBranding;
        LogoAreaTitle = logoAreaTitle;
    }
}

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
        string.Empty,
        new CarouselContent(),
        string.Empty,
        null,
        null,
        string.Empty,
        new List<ExpandingLinkBox>(),
        string.Empty,
        true
        )
    {
    }
}