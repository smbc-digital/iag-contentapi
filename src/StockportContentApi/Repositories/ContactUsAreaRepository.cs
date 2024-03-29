﻿namespace StockportContentApi.Repositories;

public class ContactUsAreaRepository
{
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulContactUsArea, ContactUsArea> _contentfulFactory;

    public ContactUsAreaRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulContactUsArea, ContactUsArea> contentfulFactory)
    {
        _client = clientManager.GetClient(config);
        _contentfulFactory = contentfulFactory;
    }

    public async Task<HttpResponse> GetContactUsArea()
    {

        var builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactUsArea").Include(3);

        var entries = await _client.GetEntries(builder);
        var entry = entries.FirstOrDefault();

        var contactUsArea = _contentfulFactory.ToModel(entry);
        if (contactUsArea == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No contact us area found");

        return HttpResponse.Successful(contactUsArea);
    }
}
