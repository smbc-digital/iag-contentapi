namespace StockportContentApi.Model;

[ExcludeFromCodeCoverage]
public class SubItem
{
    public string Slug { get; set; }
    public string Title { get; set; }
    public string Teaser { get; set; }
    public string Icon { get; set; }
    public EColourScheme ColourScheme { get; set; } = EColourScheme.None;
    public string Type { get; set; }
    public string ContentType { get; set; }
    public DateTime SunriseDate { get; set; }
    public DateTime SunsetDate { get; set; }
    public string Image { get; set; }
    public string MailingListId { get; set; }
    public string Body { get; set; }
    public string Link { get; set; }
    public string ButtonText { get; set; }
    public List<SubItem> SubItems { get; set; }
    public string Statistic { get; set; }
    public string StatisticSubheading { get; set; }


    public SubItem() { }

    public SubItem(string slug, string title, string teaser, string icon, string type, string contentType, DateTime sunriseDate, DateTime sunsetDate, string image, string mailingListId, string body, List<SubItem> subItems, string link, string buttonText, EColourScheme colourScheme, string statistic, string statisticSubheading)
    {
        Slug = slug;
        Teaser = teaser;
        Title = title;
        Icon = icon;
        Type = type;
        ContentType = contentType;
        SunriseDate = sunriseDate;
        SunsetDate = sunsetDate;
        Image = image;
        MailingListId = mailingListId;
        Body = body;
        SubItems = subItems;
        ColourScheme = colourScheme;
        Link = link;
        ButtonText = buttonText;
        Statistic = statistic;
        StatisticSubheading = statisticSubheading;
    }
}