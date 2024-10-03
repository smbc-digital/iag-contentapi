namespace StockportContentApi.Repositories;

public class FooterRepository
{
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulFooter, Footer> _contentfulFactory;

    public FooterRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulFooter, Footer> contentfulFactory)
    {
        _client = clientManager.GetClient(config);
        _contentfulFactory = contentfulFactory;
    }

    public async Task<HttpResponse> GetFooter()
    {
        QueryBuilder<ContentfulFooter> builder = new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1);

        ContentfulCollection<ContentfulFooter> entries = await _client.GetEntries(builder);
        ContentfulFooter entry = entries.FirstOrDefault();

        Footer footer = _contentfulFactory.ToModel(entry);
        
        if (footer is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No footer found");

        return HttpResponse.Successful(footer);
    }
}
