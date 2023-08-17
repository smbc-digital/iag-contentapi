﻿namespace StockportContentApi.ContentfulModels;

public class ContentfulSpotlightOnBanner : IContentfulModel
{
    public string Title { get; set; } = string.Empty;
    public Asset Image { get; set; } = null;
    public string AltText { get; set; } = string.Empty;
    public string Teaser { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public SystemProperties Sys { get; set; } = new SystemProperties();
}
