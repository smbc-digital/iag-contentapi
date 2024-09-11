namespace StockportContentApi.ContentfulFactories;

public class TriviaContentfulFactory : IContentfulFactory<ContentfulTrivia, Trivia>
{
    public Trivia ToModel(ContentfulTrivia entry) =>
        new(entry.Name, entry.Icon, entry.Body, entry.Link, entry.Statistic, entry.StatisticSubHeading);
}
