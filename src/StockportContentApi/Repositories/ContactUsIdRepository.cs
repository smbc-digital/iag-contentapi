﻿namespace StockportContentApi.Repositories;

public class ContactUsIdRepository
{
    private readonly IContentfulFactory<ContentfulContactUsId, ContactUsId> _contentfulFactory;
    private readonly Contentful.Core.IContentfulClient _client;

    public ContactUsIdRepository(ContentfulConfig config, IContentfulFactory<ContentfulContactUsId, ContactUsId> contentfulFactory, IContentfulClientManager contentfulClientManager)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
    }

    public async Task<HttpResponse> GetContactUsIds(string slug)
    {
        var builder = new QueryBuilder<ContentfulContactUsId>().ContentTypeIs("contactUsId").FieldEquals("fields.slug", slug).Include(1);

        var entries = await _client.GetEntries(builder);
        var entry = entries.FirstOrDefault();

        if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No contact us id found for '{slug}'");

        var contactUsId = _contentfulFactory.ToModel(entry);

        return HttpResponse.Successful(contactUsId);
    }
}
