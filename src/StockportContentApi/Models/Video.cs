﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Video
{
    public string Heading { get; set; }
    public string Text { get; set; }
    public string VideoEmbedCode { get; set; }

    public Video(string heading, string text, string videoEmbedCode)
    {
        Heading = heading;
        Text = text;
        VideoEmbedCode = videoEmbedCode;
    }
}