namespace StockportContentApi.Repositories;

public class OrganisationRepository
{
    private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulFactory;
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly IGroupRepository _groupRepository;

    public OrganisationRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulOrganisation, Organisation> contentfulFactory,
        IContentfulClientManager contentfulClientManager,
        IGroupRepository groupRepository)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
        _groupRepository = groupRepository;
    }

    public async Task<HttpResponse> GetOrganisation(string slug)
    {
        QueryBuilder<ContentfulOrganisation> builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);

        ContentfulCollection<ContentfulOrganisation> entries = await _client.GetEntries(builder);

        ContentfulOrganisation entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No Organisation found");

        Organisation organisation = _contentfulFactory.ToModel(entry);

        organisation.Groups = await _groupRepository.GetLinkedGroupsByOrganisation(slug);

        return HttpResponse.Successful(organisation);
    }
}