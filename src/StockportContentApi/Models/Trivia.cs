namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Trivia
{
    public string Name { get; }
    public string Icon { get; }
    public string Body { get; }
    public string Link { get; }
    public string Statistic { get; set; }
    public string StatisticSubheading { get; set; }

    public Trivia(string name, string icon, string body, string link, string statistic, string statisticSubheading)
    {
        Name = name;
        Icon = icon;
        Body = body;
        Link = link;
        Statistic = statistic;
        StatisticSubheading = statisticSubheading;
    }
}