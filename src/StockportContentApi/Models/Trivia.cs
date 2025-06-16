namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Trivia(string title,
                    string icon,
                    string body,
                    string link,
                    string statistic,
                    string statisticSubheading)
{
    public string Title { get; } = title;
    public string Icon { get; } = icon;
    public string Body { get; } = body;
    public string Link { get; } = link;
    public string Statistic { get; set; } = statistic;
    public string StatisticSubheading { get; set; } = statisticSubheading;
}