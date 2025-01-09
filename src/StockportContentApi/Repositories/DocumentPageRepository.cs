namespace StockportContentApi.Repositories;

public interface IDocumentPageRepository
{
    Task<HttpResponse> GetDocumentPage(string documentPageSlug);
}

public class DocumentPageRepository(ContentfulConfig config,
                                    IContentfulClientManager contentfulClientManager,
                                    IContentfulFactory<ContentfulDocumentPage, DocumentPage> contentfulFactory,
                                    ICache cache) : BaseRepository, IDocumentPageRepository
{
    private readonly ICache _cache = cache;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulDocumentPage, DocumentPage> _contentfulFactory = contentfulFactory;

    public async Task<HttpResponse> GetDocumentPage(string documentPageSlug)
    {
        ContentfulDocumentPage entry = await _cache.GetFromCacheOrDirectlyAsync($"documentPage-{documentPageSlug}",
            () => GetDocumentPageEntry(documentPageSlug));

        DocumentPage documentPage = entry is null
            ? null
            : _contentfulFactory.ToModel(entry);

        return documentPage is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No document page found for '{documentPageSlug}'")
            : HttpResponse.Successful(documentPage);
    }

    internal async Task<ContentfulDocumentPage> GetDocumentPageEntry(string documentPageSlug)
    {
        QueryBuilder<ContentfulDocumentPage> builder = new QueryBuilder<ContentfulDocumentPage>()
            .ContentTypeIs("documentPage")
            .FieldEquals("fields.slug", documentPageSlug)
            .Include(3);

        ContentfulCollection<ContentfulDocumentPage> entries = await _client.GetEntries(builder);
        ContentfulDocumentPage entry = entries.FirstOrDefault();

        return entry;
    }
}