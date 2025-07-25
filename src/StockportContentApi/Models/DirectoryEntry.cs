﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class DirectoryEntry
{
    public string Slug { get; set; }
    public string Name { get; set; }
    public string Provider { get; set; }
    public string Description { get; set; }
    public string Teaser { get; set; }
    public string MetaDescription { get; set; }
    public IEnumerable<FilterTheme> Themes { get; set; } = new List<FilterTheme>();
    public IEnumerable<MinimalDirectory> Directories { get; set; }
    public IEnumerable<Alert> Alerts { get; set; }
    public IEnumerable<Alert> AlertsInline { get; set; }
    public IEnumerable<TrustedLogo> TrustedLogos { get; set; } = new List<TrustedLogo>();
    public IEnumerable<string> Tags { get; set; } = new List<string>();
    public string Image { get; set; }
    public MapPosition MapPosition { get; set; } = new();
    public string PhoneNumber { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Website { get; set; } = string.Empty;
    public string Twitter { get; set; } = string.Empty;
    public string Facebook { get; set; } = string.Empty;
    public string Youtube { get; set; } = string.Empty;
    public string Instagram { get; set; } = string.Empty;
    public string LinkedIn { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}