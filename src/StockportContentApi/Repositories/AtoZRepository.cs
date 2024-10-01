﻿namespace StockportContentApi.Repositories;

public class AtoZRepository : BaseRepository
{
    private readonly DateComparer _dateComparer;
    private readonly IContentfulClient _client;
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
        List<AtoZ> atozItems = new();
        atozItems.AddRange(await GetAtoZ(letter));

        atozItems = atozItems.OrderBy(o => o.Title).ToList();

        return !atozItems.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No results found")
            : HttpResponse.Successful(atozItems);
    }

    public async Task<List<AtoZ>> GetAtoZ(string letter)
    {
        string letterToLower = letter.ToLower();

        List<AtoZ> atozItems = new();
        atozItems.AddRange(await _cache.GetFromCacheOrDirectlyAsync($"atoz-article-{letterToLower}", () => GetAtoZItemFromContentType("article", letterToLower), _atoZTimeout));
        atozItems.AddRange(await _cache.GetFromCacheOrDirectlyAsync($"atoz-topic-{letterToLower}", () => GetAtoZItemFromContentType("topic", letterToLower), _atoZTimeout));
        atozItems.AddRange(await _cache.GetFromCacheOrDirectlyAsync($"atoz-showcase-{letterToLower}", () => GetAtoZItemFromContentType("showcase", letterToLower), _atoZTimeout));
        atozItems = atozItems.OrderBy(o => o.Title).ToList();

        return atozItems;
    }

    public async Task<List<AtoZ>> GetAtoZItemFromContentType(string contentType, string letter)
    {
        List<AtoZ> atozList = new();
        QueryBuilder<ContentfulAtoZ> builder = new QueryBuilder<ContentfulAtoZ>()
            .ContentTypeIs(contentType)
            .Include(0);
        ContentfulCollection<ContentfulAtoZ> entries = await GetAllEntriesAsync(_client, builder, _logger);
        IEnumerable<ContentfulAtoZ> entriesWithDisplayOn = entries is not null
            ? entries
                .Where(x => x.DisplayOnAZ.Equals("True")
                && ((x.Title.ToLower().StartsWith(letter)) || (x.Name.ToLower().StartsWith(letter)) || (x.AlternativeTitles is null ? false : (x.AlternativeTitles.Any(alt => alt.ToLower().StartsWith(letter))))))
            : null;

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
}