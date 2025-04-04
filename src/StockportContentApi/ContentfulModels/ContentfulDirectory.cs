﻿namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulDirectory : ContentfulReference
{
    public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    public List<ContentfulAlert> AlertsInline { get; set; } = new List<ContentfulAlert>();
    public ContentfulCallToActionBanner CallToAction { get; set; }
    public List<ContentfulDirectory> SubDirectories { get; set; }
    public string SearchBranding { get; set; } = "Default";
    public ContentfulEventBanner EventBanner { get; set; } = new()
    {
        Sys = new SystemProperties { Type = "Entry" }
    };
    public List<ContentfulReference> RelatedContent { get; set; } = new List<ContentfulReference>();
    public List<ContentfulExternalLink> ExternalLinks { get; set; } = new List<ContentfulExternalLink>();
    public List<ContentfulDirectoryEntry> PinnedEntries { get; set; } = new List<ContentfulDirectoryEntry>();
}