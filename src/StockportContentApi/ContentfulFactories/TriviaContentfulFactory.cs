namespace StockportContentApi.ContentfulFactories;

public class TriviaContentfulFactory : IContentfulFactory<ContentfulTrivia, Trivia>
{
    public Trivia ToModel(ContentfulTrivia entry)
    {
        return new Trivia(entry.Name, entry.Icon, entry.Text, entry.Link);
    }
}
