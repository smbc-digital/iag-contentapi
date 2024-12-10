namespace StockportContentApi.Repositories;

public class OrganisationRepository
{
    private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulFactory;
    private readonly IContentfulClient _client;
    private readonly Func<ContentfulConfig, CacheKeyConfig, IGroupRepository> _groupRepository;

    public OrganisationRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulOrganisation, Organisation> contentfulFactory,
        IContentfulClientManager contentfulClientManager,
        Func<ContentfulConfig, CacheKeyConfig, IGroupRepository> groupRepository)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
        _groupRepository = groupRepository;
    }

    public async Task<HttpResponse> GetOrganisation(string slug, ContentfulConfig config, CacheKeyConfig cacheKeyConfig)
    {
        QueryBuilder<ContentfulOrganisation> builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);
        ContentfulCollection<ContentfulOrganisation> entries = await _client.GetEntries(builder);
        ContentfulOrganisation entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No Organisation found");

        Organisation organisation = _contentfulFactory.ToModel(entry);

        organisation.Groups = await GetGroup(slug, config, cacheKeyConfig);

        return HttpResponse.Successful(organisation);
    }

    private async Task<List<Group>> GetGroup(string groupSlug, ContentfulConfig config, CacheKeyConfig cacheKeyConfig)
    {
        IGroupRepository groupRepository = _groupRepository(config, cacheKeyConfig);
        List<Group> groups = await groupRepository.GetLinkedGroupsByOrganisation(groupSlug);

        return groups;
    }
}