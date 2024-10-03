namespace StockportContentApi.Repositories;

public class NewsRepository : BaseRepository
{
    private readonly ITimeProvider _timeProvider;
    private readonly IContentfulFactory<ContentfulNews, News> _newsContentfulFactory;
    private readonly IContentfulFactory<ContentfulNewsRoom, Newsroom> _newsRoomContentfulFactory;
    private readonly DateComparer _dateComparer;
    private readonly ICache _cache;
    private readonly IContentfulClient _client;
    private readonly IConfiguration _configuration;
    private readonly int _newsTimeout;

    public NewsRepository(ContentfulConfig config, ITimeProvider timeProvider, IContentfulClientManager contentfulClientManager,
        IContentfulFactory<ContentfulNews, News> newsContentfulFactory, IContentfulFactory<ContentfulNewsRoom, Newsroom> newsRoomContentfulFactory, ICache cache, IConfiguration configuration)
    {
        _timeProvider = timeProvider;
        _newsContentfulFactory = newsContentfulFactory;
        _newsRoomContentfulFactory = newsRoomContentfulFactory;
        _dateComparer = new DateComparer(timeProvider);
        _client = contentfulClientManager.GetClient(config);
        _cache = cache;
        _configuration = configuration;
        int.TryParse(_configuration["redisExpiryTimes:News"], out _newsTimeout);
    }

    private async Task<IList<ContentfulNews>> GetAllNews()
    {
        QueryBuilder<ContentfulNews> builder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").Include(1);
        ContentfulCollection<ContentfulNews> entries = await GetAllEntriesAsync(_client, builder);

        return !entries.Any()
            ? null
            : entries.ToList();
    }

    private async Task<ContentfulNewsRoom> GetNewsRoom()
    {
        QueryBuilder<ContentfulNewsRoom> builder = new QueryBuilder<ContentfulNewsRoom>().ContentTypeIs("newsroom").Include(1);
        ContentfulCollection<ContentfulNewsRoom> entries = await _client.GetEntries(builder);

        return entries.FirstOrDefault();
    }

    private async Task<List<string>> GetNewsCategories()
    {
        ContentType eventType = await _client.GetContentType("news");
        InValuesValidator validation = eventType.Fields.First(field => field.Name.Equals("Categories")).Items.Validations[0] as InValuesValidator;

        return validation?.RequiredValues;
    }

    public async Task<HttpResponse> GetNews(string slug)
    {
        IList<ContentfulNews> entries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);

        ContentfulNews entry = entries.Where(e => e.Slug.Equals(slug)).FirstOrDefault();

        if (entry is not null && !_dateComparer.DateNowIsAfterSunriseDate(entry.SunriseDate))
            entry = null;

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No news found for '{slug}'")
            : HttpResponse.Successful(_newsContentfulFactory.ToModel(entry));
    }

    public async Task<HttpResponse> Get(string tag, string category, DateTime? startDate, DateTime? endDate)
    {
        Newsroom newsroom = new(new List<Alert>(), false, string.Empty);
        ContentfulNewsRoom newsRoomEntry = await _cache.GetFromCacheOrDirectlyAsync("newsroom", GetNewsRoom, _newsTimeout);
        List<string> categories;

        if (newsRoomEntry is not null)
            newsroom = _newsRoomContentfulFactory.ToModel(newsRoomEntry);

        IList<ContentfulNews> newsEntries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);
        IEnumerable<ContentfulNews> filteredEntries = newsEntries.Where(news => tag is null || news.Tags.Any(t => string.Equals(t, tag, StringComparison.InvariantCultureIgnoreCase)));

        if (!filteredEntries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

        List<News> newsArticles = filteredEntries.Select(item => _newsContentfulFactory.ToModel(item))
                                    .GetNewsDates(out List<DateTime> dates, _timeProvider)
                                    .Where(news => CheckDates(startDate, endDate, news))
                                    .Where(news => string.IsNullOrWhiteSpace(category) 
                                        || news.Categories.Any(cat => string.Equals(cat, category, StringComparison.InvariantCultureIgnoreCase)))
                                    .OrderByDescending(o => o.SunriseDate)
                                    .ToList();

        categories = await GetCategories();
        newsroom.SetNews(newsArticles);
        newsroom.SetCategories(categories);
        newsroom.SetDates(dates.Distinct().ToList());

        return HttpResponse.Successful(newsroom);
    }

    private bool CheckDates(DateTime? startDate, DateTime? endDate, News news) =>
        startDate.HasValue && endDate.HasValue
            ? _dateComparer.SunriseDateIsBetweenStartAndEndDates(news.SunriseDate, startDate.Value, endDate.Value)
            : _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate);

    public async Task<HttpResponse> GetNewsByLimit(int limit)
    {
        IList<ContentfulNews> newsEntries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);

        if (!newsEntries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

        List<News> newsArticles = newsEntries.Select(item => _newsContentfulFactory.ToModel(item))
                                    .Where(news => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate))
                                    .OrderByDescending(o => o.SunriseDate)
                                    .Take(limit)
                                    .ToList();

        return !newsArticles.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No news found")
            : HttpResponse.Successful(newsArticles);
    }

    public async Task<List<string>> GetCategories() =>
        await _cache.GetFromCacheOrDirectlyAsync("news-categories", GetNewsCategories, _newsTimeout);

    public virtual async Task<News> GetLatestNewsByTag(string tag)
    {
        QueryBuilder<ContentfulNews> newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldMatches(n => n.Tags, tag).Include(1);
        ContentfulCollection<ContentfulNews> newsEntries = await _client.GetEntries(newsBuilder);
        ContentfulNews contentfulNews = newsEntries.FirstOrDefault(news => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate));

        if (contentfulNews is not null)
            return _newsContentfulFactory.ToModel(contentfulNews);

        return null;
    }

    public virtual async Task<News> GetLatestNewsByCategory(string category)
    {
        QueryBuilder<ContentfulNews> newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldMatches(n => n.Categories, category).Include(1);
        ContentfulCollection<ContentfulNews> newsEntries = await _client.GetEntries(newsBuilder);

        ContentfulNews contentfulNews = newsEntries.FirstOrDefault(news => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate));
        if (contentfulNews is not null)
            return _newsContentfulFactory.ToModel(contentfulNews);

        return null;
    }
}