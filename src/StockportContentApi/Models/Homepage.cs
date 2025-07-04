﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Homepage
{
    public string FeaturedTasksHeading { get; }
    public string FeaturedTasksSummary { get; }
    public IEnumerable<SubItem> FeaturedTasks { get; }
    public IEnumerable<SubItem> FeaturedTopics { get; }
    public IEnumerable<Alert> Alerts { get; }
    public IEnumerable<CarouselContent> CarouselContents { get; }
    public string BackgroundImage { get; }
    public string ForegroundImage { get; }
    public string ForegroundImageLocation { get; }
    public string ForegroundImageLink { get; }
    public string ForegroundImageAlt { get; }
    public string FreeText { get; }
    public string Title { get; }
    public string ImageOverlayText { get; set; }
    public string EventCategory { get; }
    public string MetaDescription { get; set; }
    public CarouselContent CampaignBanner { get; set; }
    public CallToActionBanner CallToAction { get; set; }
    public CallToActionBanner CallToActionPrimary { get; set; }
    public IEnumerable<SpotlightOnBanner> SpotlightOnBanner { get; set; }

    public Homepage(string featuredTasksHeading,
                    string featuredTasksSummary,
                    IEnumerable<SubItem> featuredTasks,
                    IEnumerable<SubItem> featuredTopics,
                    IEnumerable<Alert> alerts,
                    IEnumerable<CarouselContent> carouselContents,
                    string backgroundImage,
                    string foregroundImage,
                    string foregroundImageLocation,
                    string foregroundImageLink,
                    string foregroundImageAlt,
                    string freeText,
                    string title,
                    string eventCategory,
                    string metaDescription,
                    CarouselContent campaignBanner,
                    CallToActionBanner callToAction,
                    CallToActionBanner callToActionPrimary,
                    IEnumerable<SpotlightOnBanner> spotlightOnBanner,
                    string imageOverlayText)
    {
        FeaturedTasksHeading = featuredTasksHeading;
        FeaturedTasksSummary = featuredTasksSummary;
        FeaturedTasks = featuredTasks;
        FeaturedTopics = featuredTopics;
        Alerts = alerts;
        CarouselContents = carouselContents;
        BackgroundImage = backgroundImage;
        ForegroundImage = foregroundImage;
        ForegroundImageLocation = foregroundImageLocation;
        ForegroundImageLink = foregroundImageLink;
        ForegroundImageAlt = foregroundImageAlt;
        FreeText = freeText;
        EventCategory = eventCategory;
        MetaDescription = metaDescription;
        CampaignBanner = campaignBanner;
        CallToAction = callToAction;
        CallToActionPrimary = callToActionPrimary;
        SpotlightOnBanner = spotlightOnBanner;
        Title = title;
        ImageOverlayText = imageOverlayText;
    }
}

[ExcludeFromCodeCoverage]
public class NullHomepage : Homepage
{
    public NullHomepage() : base(string.Empty,
                                string.Empty,
                                new List<SubItem>(),
                                new List<SubItem>(),
                                new List<Alert>(),
                                new List<CarouselContent>(),
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                string.Empty,
                                new CarouselContent(),
                                new CallToActionBanner(),
                                new CallToActionBanner(),
                                null,
                                string.Empty)
    { }
}