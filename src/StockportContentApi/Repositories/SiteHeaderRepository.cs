﻿namespace StockportContentApi.Repositories;

public class SiteHeaderRepository
{
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulSiteHeader, SiteHeader> _contentfulFactory;

    public SiteHeaderRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulSiteHeader, SiteHeader> contentfulFactory)
    {
        _client = clientManager.GetClient(config);
        _contentfulFactory = contentfulFactory;
    }

    public async Task<HttpResponse> GetSiteHeader()
    {
        QueryBuilder<ContentfulSiteHeader> builder = new QueryBuilder<ContentfulSiteHeader>().ContentTypeIs("header").Include(1);

        ContentfulCollection<ContentfulSiteHeader> entries = await _client.GetEntries(builder);
        ContentfulSiteHeader entry = entries.FirstOrDefault();

        SiteHeader header = _contentfulFactory.ToModel(entry);

        if (header is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No header found");

        return HttpResponse.Successful(header);
    }
}
