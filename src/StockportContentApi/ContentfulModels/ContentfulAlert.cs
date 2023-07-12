﻿namespace StockportContentApi.ContentfulModels;

public class ContentfulAlert : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public string SubHeading { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Severity { get; set; } = string.Empty;
    public DateTime SunriseDate { get; set; } = DateTime.MinValue.ToUniversalTime();
    public DateTime SunsetDate { get; set; } = DateTime.MaxValue.ToUniversalTime();
    public SystemProperties Sys { get; set; } = new SystemProperties();
    public string Slug { get; set; } = string.Empty;
    public bool IsStatic { get; set; }
    public Asset Image { get; set; } = new Asset { File = new File { Url = "" }, SystemProperties = new SystemProperties { Type = "Asset" } };
}