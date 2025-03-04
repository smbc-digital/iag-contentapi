namespace StockportContentApi.Repositories;

public interface ISiteHeaderRepository
{
    Task<HttpResponse> GetSiteHeader();
}

public class SiteHeaderRepository(ContentfulConfig config,
                                IContentfulClientManager clientManager,
                                IContentfulFactory<ContentfulSiteHeader, SiteHeader> contentfulFactory) : ISiteHeaderRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulSiteHeader, SiteHeader> _contentfulFactory = contentfulFactory;

    public async Task<HttpResponse> GetSiteHeader()
    {
        QueryBuilder<ContentfulSiteHeader> builder = new QueryBuilder<ContentfulSiteHeader>().ContentTypeIs("header").Include(1);

        ContentfulCollection<ContentfulSiteHeader> entries = await _client.GetEntries(builder);
        ContentfulSiteHeader entry = entries.FirstOrDefault();

        SiteHeader header = entry is null
            ? null
            : _contentfulFactory.ToModel(entry);

        return header is not null
            ? HttpResponse.Successful(header)
            : HttpResponse.Failure(HttpStatusCode.NotFound, "No header found");
    }
}