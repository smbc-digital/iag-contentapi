﻿using Contentful.Core.Models;

namespace StockportContentApi.Model;

public class Homepage
{
    public IEnumerable<string> PopularSearchTerms { get; }
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
    public Group FeaturedGroup { get; }
    public string EventCategory { get; }
    public string MetaDescription { get; set; }
    public CarouselContent CampaignBanner { get; set; }
    public CallToActionBanner CallToAction {get;set;}
    public IEnumerable<SpotlightOnBanner> SpotlightOnBanner { get; set; }

    public Homepage(IEnumerable<string> popularSearchTerms, string featuredTasksHeading, string featuredTasksSummary, IEnumerable<SubItem> featuredTasks, IEnumerable<SubItem> featuredTopics, IEnumerable<Alert> alerts,
        IEnumerable<CarouselContent> carouselContents, string backgroundImage, string foregroundImage, string foregroundImageLocation, string foregroundImageLink, string foregroundImageAlt,
        string freeText, Group featuredGroup, string eventCategory, string metaDescription, CarouselContent campaignBanner, CallToActionBanner callToAction, IEnumerable<SpotlightOnBanner> spotlightOnBanner)
    {
        PopularSearchTerms = popularSearchTerms;
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
        FeaturedGroup = featuredGroup;
        EventCategory = eventCategory;
        MetaDescription = metaDescription;
        CampaignBanner = campaignBanner;
        CallToAction = callToAction;
        SpotlightOnBanner = spotlightOnBanner;
    }
}

public class NullHomepage : Homepage
{
    public NullHomepage() : base(Enumerable.Empty<string>(), string.Empty, string.Empty, new List<SubItem>(),
        new List<SubItem>(), new List<Alert>(), new List<CarouselContent>(), string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, null, string.Empty,
        string.Empty, new CarouselContent(), new CallToActionBanner(), null)
    {
    }
}
