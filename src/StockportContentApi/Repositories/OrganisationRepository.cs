namespace StockportContentApi.Repositories;

public interface IOrganisationRepository
{
    Task<HttpResponse> GetOrganisation(string slug, ContentfulConfig config, CacheKeyConfig cacheKeyConfig);
}

public class OrganisationRepository(ContentfulConfig config,
                                    IContentfulFactory<ContentfulOrganisation, Organisation> contentfulFactory,
                                    IContentfulClientManager contentfulClientManager) : IOrganisationRepository
{
    private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulFactory = contentfulFactory;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);

    public async Task<HttpResponse> GetOrganisation(string slug, ContentfulConfig config, CacheKeyConfig cacheKeyConfig)
    {
        QueryBuilder<ContentfulOrganisation> builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);
        ContentfulCollection<ContentfulOrganisation> entries = await _client.GetEntries(builder);
        ContentfulOrganisation entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No organisation found");

        Organisation organisation = _contentfulFactory.ToModel(entry);

        return HttpResponse.Successful(organisation);
    }
}