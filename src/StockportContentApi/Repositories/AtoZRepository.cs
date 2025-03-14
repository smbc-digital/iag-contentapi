namespace StockportContentApi.Repositories;

public interface IAtoZRepository
{
    Task<HttpResponse> Get(string letter = "");
}

public class AtoZRepository(
        ContentfulConfig config,
        IContentfulClientManager contentfulClientManager,
        IContentfulFactory<ContentfulAtoZ, AtoZ> contentfulAtoZFactory,
        ITimeProvider timeProvider,
        ICache cache,
        IConfiguration configuration,
        ILogger logger) : BaseRepository, IAtoZRepository
{
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulAtoZ, AtoZ> _contentfulAtoZFactory = contentfulAtoZFactory;
    private readonly DateComparer _dateComparer = new DateComparer(timeProvider);
    private readonly ICache _cache = cache;
    private readonly int _atoZTimeout = GetCacheConfiguration(configuration);
    private readonly ILogger _logger = logger;
    private List<string> contentTypesToInclude = new() { "article", "topic", "showcase", "landingPage" };

    private static int GetCacheConfiguration(IConfiguration configuration)
    {
        int timeout = 0;
        int.TryParse(configuration["redisExpiryTimes:AtoZ"], out timeout);

        return timeout;
    }

    public async Task<HttpResponse> Get(string letter = "")
    {
        IEnumerable<AtoZ> atozItems = string.IsNullOrEmpty(letter) 
                            ? await GetAtoZ() 
                            : await GetAtoZ(letter);

        return !atozItems.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No results found")
            : HttpResponse.Successful(atozItems.OrderBy(o => o.Title).ToList());
    }

    public async Task<IEnumerable<AtoZ>> GetAtoZ()
    {
        List<AtoZ> atozItems = new();
        foreach(var contentType in contentTypesToInclude)
        {
            var items = await _cache.GetFromCacheOrDirectlyAsync($"{config.BusinessId}-atoz-{contentType}", () => GetAtoZItemFromSource(contentType), _atoZTimeout);
            atozItems.AddRange(items);
        }    

        return atozItems;
    }
        
    public async Task<IEnumerable<AtoZ>> GetAtoZ(string letter)
    {
        List<AtoZ> atozItems = new();
        foreach (var contentType in contentTypesToInclude)
        {
            var items = await _cache.GetFromCacheOrDirectlyAsync($"{config.BusinessId}-atoz-{contentType}-{letter.ToLower()}", () => GetAtoZItemFromSource(contentType, letter.ToLower()), _atoZTimeout);
            if (items is not null && items.Any())
                atozItems.AddRange(items);
        }

        return atozItems;
    }

    public async Task<List<AtoZ>> GetAtoZItemFromSource(string contentType, string letter)
    {
        List<AtoZ> atozList = new();
        QueryBuilder<ContentfulAtoZ> builder = new QueryBuilder<ContentfulAtoZ>().ContentTypeIs(contentType).Include(0);
        ContentfulCollection<ContentfulAtoZ> entries = await GetAllEntriesAsync(_client, builder, _logger);

        IEnumerable<ContentfulAtoZ> entriesWithDisplayOn = entries?.Where(entry => entry.DisplayOnAZ.Equals("True")
                                                                && entry.Title.ToLower().StartsWith(letter)
                                                                || entry.Name.ToLower().StartsWith(letter) 
                                                                || (entry.AlternativeTitles is not null 
                                                                && entry.AlternativeTitles.Any(alternativeTitle => alternativeTitle.ToLower().StartsWith(letter))));

        if (entriesWithDisplayOn is not null)
        {
            foreach (ContentfulAtoZ item in entriesWithDisplayOn)
            {
                DateTime sunriseDate = DateComparer.DateFieldToDate(item.SunriseDate);
                DateTime sunsetDate = DateComparer.DateFieldToDate(item.SunsetDate);

                if (_dateComparer.DateNowIsWithinSunriseAndSunsetDates(sunriseDate, sunsetDate))
                {
                    AtoZ buildItem = _contentfulAtoZFactory.ToModel(item);
                    List<AtoZ> matchingItems = buildItem.SetTitleStartingWithLetter(letter);
                    atozList.AddRange(matchingItems);
                }
            }
        }

        return atozList;
    }

    public async Task<List<AtoZ>> GetAtoZItemFromSource(string contentType)
    {
        List<AtoZ> atozList = new();
        QueryBuilder<ContentfulAtoZ> builder = new QueryBuilder<ContentfulAtoZ>().ContentTypeIs(contentType).Include(0);
        ContentfulCollection<ContentfulAtoZ> entries = await GetAllEntriesAsync(_client, builder, _logger);

        IEnumerable<ContentfulAtoZ> entriesWithDisplayOn = entries?.Where(x => x.DisplayOnAZ.Equals("True"));

        if (entriesWithDisplayOn is not null)
        {
            foreach (ContentfulAtoZ item in entriesWithDisplayOn)
            {
                DateTime sunriseDate = DateComparer.DateFieldToDate(item.SunriseDate);
                DateTime sunsetDate = DateComparer.DateFieldToDate(item.SunsetDate);

                if (_dateComparer.DateNowIsWithinSunriseAndSunsetDates(sunriseDate, sunsetDate))
                    atozList.AddRange(_contentfulAtoZFactory.ToModel(item).SetTitles());
            }
        }

        return atozList;
    }
}