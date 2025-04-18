﻿namespace StockportContentApi.ContentfulModels;

/// <summary>
/// Contentful reference base class, used for anything that is generically referenced
/// </summary>
public class ContentfulReference : IContentfulModel
{
    public string Slug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string NavigationTitle { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public Asset TeaserImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public string MetaDescription { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public EColourScheme ColourScheme { get; set; } = EColourScheme.None;
    public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public bool Highlight { get; set; } = false;
    public bool HideLastUpdated { get; set; } = false;
    public Asset Image { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public Asset BackgroundImage { get; set; } = new() { File = new File { Url = string.Empty }, SystemProperties = new SystemProperties { Type = "Asset" } };
    public List<ContentfulReference> SubItems { get; set; } = new();
    public List<ContentfulReference> SecondaryItems { get; set; } = new();
    public List<ContentfulReference> TertiaryItems { get; set; } = new();
    public List<ContentfulSection> Sections { get; set; } = new();
    public SystemProperties Sys { get; set; } = new();
    public string ContentType { get; set; } = string.Empty;
    public string MailingListId { get; set; }
    public string Body { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string ButtonText { get; set; } = string.Empty;
    public string Statistic { get; set; } = string.Empty;
    public string StatisticSubheading { get; set; } = string.Empty;
    public string VideoTitle { get; set; } = string.Empty;
    public string VideoToken { get; set; } = string.Empty;
    public string VideoPlaceholderPhotoId { get; set; } = string.Empty;
    public string AssociatedTagCategory { get; set; } = string.Empty;
    public string ScreenReader { get; set; } = string.Empty;
    public string AccountName { get; set; }
}