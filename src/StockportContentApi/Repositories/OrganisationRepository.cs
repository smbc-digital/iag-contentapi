﻿namespace StockportContentApi.Repositories;

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
        var builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);

        var entries = await _client.GetEntries(builder);

        var entry = entries.FirstOrDefault();

        if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No Organisation found");

        var organisation = _contentfulFactory.ToModel(entry);

        organisation.Groups = await _groupRepository.GetLinkedGroupsByOrganisation(slug);

        return HttpResponse.Successful(organisation);
    }
}