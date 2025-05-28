namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Trivia
{
    public string Title { get; }
    public string Icon { get; }
    public string BodyText { get; }
    public string Link { get; }
    public string Statistic { get; set; }
    public string StatisticSubheading { get; set; }

    public Trivia(string title, string icon, string bodyText, string link, string statistic, string statisticSubheading)
    {
        Title = title;
        Icon = icon;
        BodyText = bodyText;
        Link = link;
        Statistic = statistic;
        StatisticSubheading = statisticSubheading;
    }
}