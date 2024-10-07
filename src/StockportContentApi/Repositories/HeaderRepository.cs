using StockportContentApi.ContentfulFactories;
using System.Reflection.PortableExecutable;

namespace StockportContentApi.Repositories;

public class HeaderRepository
{
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulHeader, Header> _contentfulFactory;

    public HeaderRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulHeader, Header> contentfulFactory)
    {
        _client = clientManager.GetClient(config);
        _contentfulFactory = contentfulFactory;
    }

    public async Task<HttpResponse> GetHeader()
    {
        QueryBuilder<ContentfulHeader> builder = new QueryBuilder<ContentfulHeader>().ContentTypeIs("header").Include(1);

        ContentfulCollection<ContentfulHeader> entries = await _client.GetEntries(builder);
        ContentfulHeader entry = entries.FirstOrDefault();

        Header header = _contentfulFactory.ToModel(entry);

        if (header is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No header found");

        return HttpResponse.Successful(header);
    }
}
