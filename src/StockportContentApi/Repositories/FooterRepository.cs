namespace StockportContentApi.Repositories;

public interface IFooterRepository
{
    Task<HttpResponse> GetFooter();
}

public class FooterRepository(ContentfulConfig config,
                            IContentfulClientManager clientManager,
                            IContentfulFactory<ContentfulFooter, Footer> contentfulFactory) : IFooterRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulFooter, Footer> _contentfulFactory = contentfulFactory;

    public async Task<HttpResponse> GetFooter()
    {
        QueryBuilder<ContentfulFooter> builder = new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1);
        ContentfulCollection<ContentfulFooter> entries = await _client.GetEntries(builder);
        ContentfulFooter entry = entries.FirstOrDefault();

        Footer footer = entry is not null
                        ? _contentfulFactory.ToModel(entry)
                        : null;
        
        return footer is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No footer found")
            : HttpResponse.Successful(footer);
    }
}