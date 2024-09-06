namespace StockportContentApi.ContentfulFactories;

public class TriviaContentfulFactory : IContentfulFactory<ContentfulReference, Trivia>
{
    public Trivia ToModel(ContentfulReference entry)
    {
        return new Trivia(entry.Name, entry.Icon, entry.Body, entry.Link);
    }
}
