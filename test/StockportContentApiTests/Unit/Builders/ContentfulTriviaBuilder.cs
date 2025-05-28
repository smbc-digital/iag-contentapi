namespace StockportContentApiTests.Unit.Builders;

public class ContentfulTriviaBuilder
{
    private string _title= "title";
    private string _icon = "icon";
    private string _body = "body";
    private string _link = "link";
    private string _statistic = "statistic";
    private string _statisticSubHeading = "statistic sub heading";

    public ContentfulTrivia Build()
        => new()
        {
            Title = _title,
            Icon = _icon,
            BodyText = _body,
            Link = _link,
            Statistic = _statistic,
            StatisticSubHeading = _statisticSubHeading,
        };
    
    public ContentfulTriviaBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulTriviaBuilder WithIcon(string icon)
    {
        _icon = icon;
        return this;
    }

    public ContentfulTriviaBuilder WithBody(string body)
    {
        _body = body;
        return this;
    }

    public ContentfulTriviaBuilder WithLink(string link)
    {
        _link = link;
        return this;
    }

    public ContentfulTriviaBuilder WithStatistic(string statistic)
    {
        _statistic = statistic;
        return this;
    }
    
    public ContentfulTriviaBuilder WithStatisticSubHeading(string statisticSubHeading)
    {
        _statisticSubHeading = statisticSubHeading;
        return this;
    }
}