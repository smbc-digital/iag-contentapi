namespace StockportContentApi.Repositories;

public class CommsRepository
{
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulCommsHomepage, CommsHomepage> _commsHomepageFactory;

    public CommsRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulCommsHomepage, CommsHomepage> commsHomepageFactory)
    {
        _client = clientManager.GetClient(config);
        _commsHomepageFactory = commsHomepageFactory;
    }

    public async Task<HttpResponse> Get()
    {
        var builder = new QueryBuilder<ContentfulCommsHomepage>().ContentTypeIs("commsHomepage").Include(1);
        var entries = await _client.GetEntries(builder);
        var entry = entries.FirstOrDefault();

        if (entry != null && entry.WhatsOnInStockportEvent == null)
        {
            var sortOrder = SortOrderBuilder<ContentfulEvent>.New(f => f.EventDate);
            var eventQueryBuilder = new QueryBuilder<ContentfulEvent>()
                .FieldGreaterThan(f => f.EventDate, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK"))
                .ContentTypeIs("events")
                .OrderBy(sortOrder.Build())
                .Limit(1);
            var eventEntry = await _client.GetEntries(eventQueryBuilder);
            entry.WhatsOnInStockportEvent = eventEntry.FirstOrDefault();
        }

        return entry == null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No comms homepage found")
            : HttpResponse.Successful(_commsHomepageFactory.ToModel(entry));
    }
}
