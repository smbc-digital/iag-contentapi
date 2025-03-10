﻿namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class SubItem
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Teaser { get; set; }
    public string TeaserImage { get; set; }
    public string Icon { get; set; }
    public EColourScheme ColourScheme { get; set; } = EColourScheme.Multi;
    public string Type { get; set; }
    public DateTime SunriseDate { get; set; }
    public DateTime SunsetDate { get; set; }
    public string Image { get; set; }
    public List<SubItem> SubItems { get; set; }

    public SubItem() { }

    public SubItem(string slug, string title, string teaser, string teaserImage, string icon, string type, DateTime sunriseDate, DateTime sunsetDate, string image, List<SubItem> subItems, EColourScheme colourScheme)
    {
        Slug = slug;
        Teaser = teaser;
        TeaserImage = teaserImage;
        Title = title;
        Icon = icon;
        Type = type;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
        Image = image;
        SubItems = subItems;
        ColourScheme = colourScheme;
    }
}