﻿namespace StockportContentApi.ContentfulModels;

[ExcludeFromCodeCoverage]
public class ContentfulVideo : IContentfulModel
{
    public string Heading { get; set; }
    public string Text { get; set; }
    public string VideoEmbedCode { get; set; }
    public SystemProperties Sys { get; set; }
}