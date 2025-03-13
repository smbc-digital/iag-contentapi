namespace StockportContentApi.Repositories;

public interface IOrganisationRepository
{
    Task<HttpResponse> GetOrganisation(string slug, ContentfulConfig config, CacheKeyConfig cacheKeyConfig);
}

public class OrganisationRepository(ContentfulConfig config,
                                    IContentfulFactory<ContentfulOrganisation, Organisation> contentfulFactory,
                                    IContentfulClientManager contentfulClientManager,
                                    Func<string, string, IGroupRepository> groupRepository) : IOrganisationRepository
{
    private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulFactory = contentfulFactory;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly Func<string, string, IGroupRepository> _groupRepository = groupRepository;

    public async Task<HttpResponse> GetOrganisation(string slug, ContentfulConfig config, CacheKeyConfig cacheKeyConfig)
    {
        QueryBuilder<ContentfulOrganisation> builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);
        ContentfulCollection<ContentfulOrganisation> entries = await _client.GetEntries(builder);
        ContentfulOrganisation entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No organisation found");

        Organisation organisation = _contentfulFactory.ToModel(entry);

        organisation.Groups = await GetGroup(slug, config, cacheKeyConfig);

        return HttpResponse.Successful(organisation);
    }

    private async Task<List<Group>> GetGroup(string groupSlug, ContentfulConfig config, CacheKeyConfig cacheKeyConfig)
    {
        IGroupRepository groupRepository = _groupRepository(config.BusinessId, cacheKeyConfig.BusinessId);
        List<Group> groups = await groupRepository.GetLinkedGroupsByOrganisation(groupSlug);

        return groups;
    }
}