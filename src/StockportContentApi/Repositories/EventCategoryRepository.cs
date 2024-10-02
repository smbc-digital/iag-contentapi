namespace StockportContentApi.Repositories;

public class EventCategoryRepository
{
    private readonly ICache _cache;
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulEventCategory, EventCategory> _contentfulFactory;
    private readonly int _eventsCategoryTimeout;
    private readonly IConfiguration _configuration;

    public EventCategoryRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulEventCategory, EventCategory> contentfulFactory,
        IContentfulClientManager contentfulClientManager, ICache cache, IConfiguration configuration)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
        _configuration = configuration;
        int.TryParse(_configuration["redisExpiryTimes:Events"], out _eventsCategoryTimeout);
        _cache = cache;
    }

    public async Task<HttpResponse> GetEventCategories()
    {
        List<EventCategory> categories = await _cache.GetFromCacheOrDirectlyAsync("event-categories-content-type",
            GetCategoriesDirect, _eventsCategoryTimeout);

        if (categories is not null && !categories.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories returned");

        categories = categories.OrderBy(category => category.Name).ToList();

        return HttpResponse.Successful(categories);
    }

    private async Task<List<EventCategory>> GetCategoriesDirect()
    {
        QueryBuilder<ContentfulEventCategory> builder =
            new QueryBuilder<ContentfulEventCategory>().ContentTypeIs("eventCategory");

        ContentfulCollection<ContentfulEventCategory> entries = await _client.GetEntries(builder);

        if (entries is null || !entries.Any())
            new List<EventCategory>();

        List<EventCategory> eventCategories =
            entries.Select(eventCatogory => _contentfulFactory.ToModel(eventCatogory)).ToList();

        return !eventCategories.Any()
            ? null
            : eventCategories;
    }
}