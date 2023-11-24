﻿namespace StockportContentApi.ContentfulModels;

public class ContentfulTopic : ContentfulReference
{
    public string Summary { get; set; } = string.Empty;
    public List<ContentfulAlert> Alerts { get; set; } = new();
    public List<ContentfulReference> Breadcrumbs { get; set; } = new();
    public string VideoTitle { get; init; }
    public string VideoTeaser { get; init; }
    public string VideoTag { get; init; }
    public string TriviaSubheading { get; set; } = string.Empty;
    public List<ContentfulTrivia> TriviaSection { get; init; }
    public ContentfulCallToActionBanner CallToAction { get; init; }
    public bool EmailAlerts { get; set; } = false;
    public string EmailAlertsTopicId { get; set; } = string.Empty;
    public ContentfulEventBanner EventBanner { get; set; } = new()
    {
        Sys = new SystemProperties { Type = "Entry" }
    };
    public bool DisplayContactUs { get; set; } = true;
    public string EventCategory { get; set; }
    public ContentfulCarouselContent CampaignBanner { get; set; } = new();
    public List<ContentfulGroupBranding> TopicBranding { get; set; } = new();
    public string LogoAreaTitle { get; set;}
}