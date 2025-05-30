﻿namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulDirectoryEntry : ContentfulReference
{
    public string Description { get; set; }
    public IEnumerable<ContentfulFilter> Filters { get; set; }
    public IEnumerable<ContentfulDirectory> Directories { get; set; }
    public MapPosition MapPosition { get; set; } = new MapPosition();
    public IEnumerable<ContentfulAlert> Alerts { get; set; }
    public IEnumerable<ContentfulAlert> AlertsInline { get; set; }
    public List<ContentfulGroupBranding> GroupBranding { get; set; } = new(); // TODO : This should be renamed at some point to just "Branding" or "Logo"
    public List<string> Tags { get; set; } = new List<string>();
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Youtube { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Provider { get; set; } = string.Empty;
}