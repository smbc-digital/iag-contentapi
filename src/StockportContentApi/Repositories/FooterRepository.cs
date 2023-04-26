namespace StockportContentApi.Repositories;

public class FooterRepository
{
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulFooter, Footer> _contentfulFactory;

    public FooterRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulFooter, Footer> contentfulFactory)
    {
        _client = clientManager.GetClient(config);
        _contentfulFactory = contentfulFactory;
    }

    public async Task<HttpResponse> GetFooter()
    {

        var builder = new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1);

        var entries = await _client.GetEntries(builder);
        var entry = entries.FirstOrDefault();

        var footer = _contentfulFactory.ToModel(entry);
        if (footer == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No footer found");

        return HttpResponse.Successful(footer);
    }
}
