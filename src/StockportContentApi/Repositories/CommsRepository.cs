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
        QueryBuilder<ContentfulCommsHomepage> builder = new QueryBuilder<ContentfulCommsHomepage>().ContentTypeIs("commsHomepage").Include(1);
        ContentfulCollection<ContentfulCommsHomepage> entries = await _client.GetEntries(builder);
        ContentfulCommsHomepage entry = entries.FirstOrDefault();

        if (entry is not null && entry.WhatsOnInStockportEvent is null)
        {
            SortOrderBuilder<ContentfulEvent> sortOrder = SortOrderBuilder<ContentfulEvent>.New(evnt => evnt.EventDate);
            QueryBuilder<ContentfulEvent> eventQueryBuilder = new QueryBuilder<ContentfulEvent>()
                .FieldGreaterThan(f => f.EventDate, DateTime.Now.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssK"))
                .ContentTypeIs("events")
                .OrderBy(sortOrder.Build())
                .Limit(1);

            ContentfulCollection<ContentfulEvent> eventEntry = await _client.GetEntries(eventQueryBuilder);
            entry.WhatsOnInStockportEvent = eventEntry.FirstOrDefault();
        }

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No comms homepage found")
            : HttpResponse.Successful(_commsHomepageFactory.ToModel(entry));
    }
}
