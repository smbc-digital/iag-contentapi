namespace StockportContentApi.Repositories;

public interface INewsRepository
{
    Task<HttpResponse> GetNews(string slug);
    Task<HttpResponse> Get(string tag, string category, DateTime? startDate, DateTime? endDate);
    Task<HttpResponse> GetNewsByLimit(int limit);
    Task<List<string>> GetCategories();
    Task<List<News>> GetLatestNewsByTag(string tag, int quantity = 1);
    Task<List<News>> GetLatestNewsByCategory(string category, int quantity = 1);
    Task<HttpResponse> GetArchivedNews(string tag, string category, DateTime? startDate, DateTime? endDate);
}

public class NewsRepository : BaseRepository, INewsRepository
{
    private readonly ITimeProvider _timeProvider;
    private readonly IContentfulFactory<ContentfulNews, News> _newsContentfulFactory;
    private readonly IContentfulFactory<ContentfulNewsRoom, Newsroom> _newsRoomContentfulFactory;
    private readonly DateComparer _dateComparer;
    private readonly ICache _cache;
    private readonly IContentfulClient _client;
    private readonly IConfiguration _configuration;
    private readonly int _newsTimeout;
    private readonly EventRepository _eventRepository;

    public NewsRepository(ContentfulConfig config,
                        ITimeProvider timeProvider,
                        IContentfulClientManager contentfulClientManager,
                        IContentfulFactory<ContentfulNews, News> newsContentfulFactory,
                        IContentfulFactory<ContentfulNewsRoom, Newsroom> newsRoomContentfulFactory,
                        ICache cache,
                        IConfiguration configuration,
                        EventRepository eventRepository)
    {
        _timeProvider = timeProvider;
        _newsContentfulFactory = newsContentfulFactory;
        _newsRoomContentfulFactory = newsRoomContentfulFactory;
        _dateComparer = new DateComparer(timeProvider);
        _client = contentfulClientManager.GetClient(config);
        _cache = cache;
        _configuration = configuration;
        _eventRepository = eventRepository;

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
        
        if (!string.IsNullOrEmpty(entry?.EventsByTagOrCategory))
        {
            IEnumerable<string> tagsOrCategories = entry?.EventsByTagOrCategory.Split(',').Select(tag => tag.Trim());
            List<Event> events = new();

            IEnumerable<Task<List<Event>>> tasks = tagsOrCategories?.Select(async tagOrCategory =>
            {
                List<Event> categoryEvents = await _eventRepository.GetEventsByCategory(tagOrCategory, true);
                return categoryEvents.Any()
                    ? categoryEvents
                    : await _eventRepository.GetEventsByTag(tagOrCategory, true);
            });

            foreach (List<Event> task in await Task.WhenAll(tasks))
            {
                events.AddRange(task);
            }

            News newsArticle = _newsContentfulFactory.ToModel(entry);

            newsArticle.Events = events
                .GroupBy(e => e.Slug)
                .Select(g => g.First())
                .OrderBy(e => e.EventDate)
                .ThenBy(e => TimeSpan.Parse(e.StartTime))
                .ThenBy(e => e.Title)
                .Take(3)
                .ToList();
        }
        
        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No news found for '{slug}'")
            : HttpResponse.Successful(_newsContentfulFactory.ToModel(entry));
    }

    public async Task<HttpResponse> Get(string tag, string category, DateTime? startDate, DateTime? endDate)
    {
        Newsroom newsroom = new(new List<Alert>(), false, string.Empty, null);
        ContentfulNewsRoom newsRoomEntry = await _cache.GetFromCacheOrDirectlyAsync("newsroom", GetNewsRoom, _newsTimeout);
        List<string> categories;

        if (newsRoomEntry is not null)
            newsroom = _newsRoomContentfulFactory.ToModel(newsRoomEntry);

        IList<ContentfulNews> newsEntries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);
        IEnumerable<ContentfulNews> filteredEntries = newsEntries
            .Where(news => tag is null || news.Tags.Any(t => string.Equals(t, tag, StringComparison.InvariantCultureIgnoreCase)))
            .Where(news => !news.SunriseDate.Equals(DateTime.MinValue) && !news.SunsetDate.Equals(DateTime.MinValue));

        if (!filteredEntries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

        List<News> newsArticles = filteredEntries.Select(_newsContentfulFactory.ToModel)
                                    .GetNewsDates(out List<DateTime> dates, _timeProvider)
                                    .GetNewsYears(out List<int> years, _timeProvider)
                                    .Where(news => CheckDates(startDate, endDate, news))
                                    .Where(news => string.IsNullOrWhiteSpace(category) 
                                        || news.Categories.Any(cat => string.Equals(cat, category, StringComparison.InvariantCultureIgnoreCase)))
                                    .OrderByDescending(o => o.SunriseDate)
                                    .ToList();

        if (newsRoomEntry.FeaturedNews is not null
            && !newsRoomEntry.FeaturedNews.SunriseDate.Equals(DateTime.MinValue)
            && !newsRoomEntry.FeaturedNews.SunsetDate.Equals(DateTime.MinValue)
            && _dateComparer.DateNowIsWithinSunriseAndSunsetDates(newsRoomEntry.FeaturedNews.SunriseDate, newsRoomEntry.FeaturedNews.SunsetDate))
        {
            newsroom.FeaturedNews = _newsContentfulFactory.ToModel(newsRoomEntry.FeaturedNews);
        }

        categories = await GetCategories();
        newsroom.SetNews(newsArticles);
        newsroom.SetCategories(categories);
        newsroom.SetDates(dates.Distinct().ToList());
        newsroom.SetYears(years);

        return HttpResponse.Successful(newsroom);
    }

    private bool CheckDates(DateTime? startDate, DateTime? endDate, News news) =>
        startDate.HasValue && endDate.HasValue
            ? _dateComparer.SunriseDateIsBetweenStartAndEndDates(news.SunriseDate, startDate.Value, endDate.Value)
            : _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate);

    private bool CheckArchivedDates(DateTime? startDate, DateTime? endDate, News news) =>
        startDate.HasValue && endDate.HasValue
            ? _dateComparer.SunriseDateIsBetweenStartAndEndDates(news.SunriseDate, startDate.Value, endDate.Value) && _dateComparer.SunsetDateIsInThePast(news.SunsetDate)
            : _dateComparer.SunsetDateIsInThePast(news.SunsetDate);

    public async Task<HttpResponse> GetNewsByLimit(int limit)
    {
        IList<ContentfulNews> newsEntries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);

        if (newsEntries is null || !newsEntries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

        List<News> newsArticles = newsEntries.Select(_newsContentfulFactory.ToModel)
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

    public virtual async Task<List<News>> GetLatestNewsByTag(string tag, int quantity = 1)
    {
        QueryBuilder<ContentfulNews> newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldMatches(n => n.Tags, tag).Include(1);
        ContentfulCollection<ContentfulNews> newsEntries = await _client.GetEntries(newsBuilder);
        List<ContentfulNews> contentfulNews = newsEntries
                                                .Where(news => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate))
                                                .OrderByDescending(n => n.SunriseDate)
                                                .Take(quantity)
                                                .ToList();

        if (!contentfulNews.Any())
            return null;
        
        List<News> newsModels = contentfulNews.Select(_newsContentfulFactory.ToModel).ToList();

        return newsModels;
    }

    public virtual async Task<List<News>> GetLatestNewsByCategory(string category, int quantity = 1)
    {
        QueryBuilder<ContentfulNews> newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldMatches(n => n.Categories, category).Include(1);
        ContentfulCollection<ContentfulNews> newsEntries = await _client.GetEntries(newsBuilder);
        List<ContentfulNews> contentfulNews = newsEntries
                                                .Where(news => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(news.SunriseDate, news.SunsetDate))
                                                .OrderByDescending(n => n.SunriseDate)
                                                .Take(quantity)
                                                .ToList();

        if (!contentfulNews.Any())
            return null;

        List<News> newsModels = contentfulNews.Select(_newsContentfulFactory.ToModel).ToList();

        return newsModels;
    }

    public async Task<HttpResponse> GetArchivedNews(string tag, string category, DateTime? startDate, DateTime? endDate)
    {
        Newsroom newsroom = new(new List<Alert>(), false, string.Empty, null);
        IList<ContentfulNews> newsEntries = await _cache.GetFromCacheOrDirectlyAsync("news-all", GetAllNews, _newsTimeout);
        List<string> categories;

        if (newsEntries is null || !newsEntries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No news found");

        IEnumerable<ContentfulNews> filteredEntries = newsEntries
            .Where(news => tag is null || news.Tags.Any(t => string.Equals(t, tag, StringComparison.InvariantCultureIgnoreCase)))
            .Where(news => !news.SunriseDate.Equals(DateTime.MinValue) && !news.SunsetDate.Equals(DateTime.MinValue))
            .Where(news => news.SunriseDate <= _timeProvider.Now());


        List<News> archivedNewsEntries = filteredEntries
            .Select(_newsContentfulFactory.ToModel)
            .GetNewsDates(out List<DateTime> dates, _timeProvider)
            .GetNewsYears(out List<int> years, _timeProvider)
            .Where(news => CheckArchivedDates(startDate, endDate, news))
            .Where(news => string.IsNullOrWhiteSpace(category)
                        || news.Categories.Any(cat => string.Equals(cat, category, StringComparison.InvariantCultureIgnoreCase)))
            .OrderByDescending(n => n.SunriseDate)
            .ToList();

        categories = await GetCategories();
        newsroom.SetNews(archivedNewsEntries);
        newsroom.SetCategories(categories);
        newsroom.SetDates(dates.Distinct().ToList());
        newsroom.SetYears(years);

        return HttpResponse.Successful(newsroom);
    }
}