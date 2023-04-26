namespace StockportContentApi.Repositories;

public class AtoZRepository : BaseRepository
{
    private readonly DateComparer _dateComparer;
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulAtoZ, AtoZ> _contentfulAtoZFactory;
    private readonly ICache _cache;
    private readonly int _atoZTimeout;
    private readonly ILogger _logger;
    private IConfiguration _configuration;

    public AtoZRepository(
        ContentfulConfig config,
        IContentfulClientManager clientManager,
        IContentfulFactory<ContentfulAtoZ, AtoZ> contentfulAtoZFactory,
        ITimeProvider timeProvider,
        ICache cache,
        IConfiguration configuration,
        ILogger logger)
    {
        _client = clientManager.GetClient(config);
        _contentfulAtoZFactory = contentfulAtoZFactory;
        _dateComparer = new DateComparer(timeProvider);
        _cache = cache;
        _configuration = configuration;
        _logger = logger;
        int.TryParse(_configuration["redisExpiryTimes:AtoZ"], out _atoZTimeout);
    }

    public async Task<HttpResponse> Get(string letter)
    {
        var atozItems = new List<AtoZ>();
        atozItems.AddRange(await GetAtoZ(letter));

        atozItems = atozItems.OrderBy(o => o.Title).ToList();

        return !atozItems.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No results found")
            : HttpResponse.Successful(atozItems);
    }

    public async Task<List<AtoZ>> GetAtoZ(string letter)
    {
        var letterToLower = letter.ToLower();

        var atozItems = new List<AtoZ>();
        atozItems.AddRange(await _cache.GetFromCacheOrDirectlyAsync("atoz-article-" + letterToLower, () => GetAtoZItemFromContentType("article", letterToLower), _atoZTimeout));
        atozItems.AddRange(await _cache.GetFromCacheOrDirectlyAsync("atoz-topic-" + letterToLower, () => GetAtoZItemFromContentType("topic", letterToLower), _atoZTimeout));
        atozItems.AddRange(await _cache.GetFromCacheOrDirectlyAsync("atoz-showcase-" + letterToLower, () => GetAtoZItemFromContentType("showcase", letterToLower), _atoZTimeout));
        atozItems = atozItems.OrderBy(o => o.Title).ToList();
        return atozItems;
    }

    public async Task<List<AtoZ>> GetAtoZItemFromContentType(string contentType, string letter)
    {
        var atozList = new List<AtoZ>();
        var builder = new QueryBuilder<ContentfulAtoZ>()
            .ContentTypeIs(contentType)
            .Include(0);
        var entries = await GetAllEntriesAsync(_client, builder, _logger);
        var entriesWithDisplayOn = entries != null ? entries
            .Where(x => x.DisplayOnAZ == "True"
            && ((x.Title.ToLower().StartsWith(letter)) || (x.Name.ToLower().StartsWith(letter)) || (x.AlternativeTitles == null ? false : (x.AlternativeTitles.Any(alt => alt.ToLower().StartsWith(letter)))))) : null;

        if (entriesWithDisplayOn != null)
        {
            foreach (var item in entriesWithDisplayOn)
            {
                DateTime sunriseDate = DateComparer.DateFieldToDate(item.SunriseDate);
                DateTime sunsetDate = DateComparer.DateFieldToDate(item.SunsetDate);
                if (_dateComparer.DateNowIsWithinSunriseAndSunsetDates(sunriseDate, sunsetDate))
                {
                    AtoZ buildItem = _contentfulAtoZFactory.ToModel(item);
                    var matchingItems = buildItem.SetTitleStartingWithLetter(letter);
                    atozList.AddRange(matchingItems);
                }
            }
        }

        return atozList;
    }
}
