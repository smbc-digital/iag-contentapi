namespace StockportContentApi.Repositories;

public interface IShowcaseRepository
{
    Task<HttpResponse> Get();
    Task<HttpResponse> GetShowcases(string slug);
}

public class ShowcaseRepository(ContentfulConfig config,
                                IContentfulFactory<ContentfulShowcase, Showcase> showcaseBuilder,
                                IContentfulClientManager contentfulClientManager,
                                IContentfulFactory<ContentfulNews, News> newsBuilder,
                                EventRepository eventRepository,
                                ILogger<ShowcaseRepository> logger) : IShowcaseRepository
{
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulShowcase, Showcase> _contentfulFactory = showcaseBuilder;
    private readonly EventRepository _eventRepository = eventRepository;
    private readonly ILogger<ShowcaseRepository> _logger = logger;
    private readonly IContentfulFactory<ContentfulNews, News> _newsFactory = newsBuilder;

    public async Task<HttpResponse> Get()
    {
        QueryBuilder<ContentfulShowcase> builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").Include(3);
        ContentfulCollection<ContentfulShowcase> entries = await _client.GetEntries(builder);
        IEnumerable<Showcase> showcases = entries.Select(_contentfulFactory.ToModel);

        return showcases.GetType().Equals(typeof(NullHomepage))
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No showcases found")
            : HttpResponse.Successful(showcases);
    }

    public async Task<HttpResponse> GetShowcases(string slug)
    {
        QueryBuilder<ContentfulShowcase> builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);
        ContentfulCollection<ContentfulShowcase> entries = await _client.GetEntries(builder);
        ContentfulShowcase entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No showcase found");

        Showcase showcase = new();

        try
        {
            showcase = _contentfulFactory.ToModel(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to serialize showcase {slug}: {ex.Message}");
        }

        if (!string.IsNullOrEmpty(showcase.EventCategory))
        {
            List<Event> events = await _eventRepository.GetEventsByCategory(showcase.EventCategory, true);

            if (!events.Any())
            {
                events = await _eventRepository.GetEventsByTag(showcase.EventCategory, true);
                if (events.Any())
                    showcase.EventsCategoryOrTag = "T";
            }

            showcase.Events = events.Take(3);
        }

        ShowcaseNews news = await PopulateNews(showcase.NewsCategoryTag);

        if (news is not null)
        {
            showcase.NewsArticle = news.News;
            showcase.NewsCategoryOrTag = news.Type;
        }

        return showcase.GetType().Equals(typeof(NullHomepage))
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No showcase found")
            : HttpResponse.Successful(showcase);
    }

    private async Task<ShowcaseNews> PopulateNews(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
            return null;

        News result = null;
        string type = string.Empty;

        QueryBuilder<ContentfulNews> newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news").FieldMatches(n => n.Categories, tag).Include(1);
        ContentfulCollection<ContentfulNews> newsEntry = await _client.GetEntries(newsBuilder);

        if (newsEntry is not null && newsEntry.Any())
            type = "C";
        else
        {
            newsBuilder =
                new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                    .FieldMatches(n => n.Tags, tag)
                    .Include(1);
            newsEntry = await _client.GetEntries(newsBuilder);

            if (newsEntry is not null && newsEntry.Any())
                type = "T";
        }

        if (newsEntry is not null && newsEntry.Any())
        {
            DateTime now = DateTime.Now.AddMinutes(5);
            ContentfulNews article = newsEntry.Where(entry => now > DateTime.Parse(entry.SunriseDate))
                .Where(entry => now < entry.SunsetDate)
                .OrderByDescending(news => news.SunriseDate)
                .Take(1)
                .FirstOrDefault();

            if (article is not null)
                result = _newsFactory.ToModel(article);
        }

        return new() { News = result, Type = type };
    }
}