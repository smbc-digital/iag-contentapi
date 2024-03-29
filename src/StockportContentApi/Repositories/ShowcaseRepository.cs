﻿namespace StockportContentApi.Repositories;

public class ShowcaseRepository
{
    private readonly IContentfulFactory<ContentfulShowcase, Showcase> _contentfulFactory;
    private readonly IContentfulFactory<ContentfulNews, News> _newsFactory;
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly EventRepository _eventRepository;
    private readonly ILogger<ShowcaseRepository> _logger;

    public ShowcaseRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulShowcase, Showcase> showcaseBuilder,
        IContentfulClientManager contentfulClientManager,
        IContentfulFactory<ContentfulNews, News> newsBuilder,
        EventRepository eventRepository,
        ILogger<ShowcaseRepository> logger)
    {
        _contentfulFactory = showcaseBuilder;
        _newsFactory = newsBuilder;
        _client = contentfulClientManager.GetClient(config);
        _eventRepository = eventRepository;
        _logger = logger;
    }
    public async Task<HttpResponse> Get()
    {
        var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").Include(3);

        var entries = await _client.GetEntries(builder);
        var showcases = entries.Select(e => _contentfulFactory.ToModel(e));

        return showcases.GetType() == typeof(NullHomepage)
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Showcases found")
            : HttpResponse.Successful(showcases);
    }

    public async Task<HttpResponse> GetShowcases(string slug)
    {
        var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);


        var entries = await _client.GetEntries(builder);

        var entry = entries.FirstOrDefault();

        if (entry == null)
        {
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No Showcase found");
        }

        Showcase showcase = new Showcase();

        try
        {
            showcase = _contentfulFactory.ToModel(entry);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Unable to serialize Showcase {slug}: {ex.Message}");
        }

        if (showcase.EventCategory != string.Empty)
        {
            var events = await _eventRepository.GetEventsByCategory(showcase.EventCategory, true);

            if (!events.Any())
            {
                events = await _eventRepository.GetEventsByTag(showcase.EventCategory, true);
                if (events.Any())
                {
                    showcase.EventsCategoryOrTag = "T";
                }
            }

            showcase.Events = events.Take(3);
        }

        var news = await PopulateNews(showcase.NewsCategoryTag);
        if (news != null)
        {
            showcase.NewsArticle = news.News;
            showcase.NewsCategoryOrTag = news.Type;
        }

        return showcase.GetType() == typeof(NullHomepage)
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Showcase found")
            : HttpResponse.Successful(showcase);
    }

    private async Task<ShowcaseNews> PopulateNews(string tag)
    {
        if (string.IsNullOrWhiteSpace(tag))
        {
            return null;
        }

        News result = null;
        var type = string.Empty;

        var newsBuilder =
            new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                .FieldMatches(n => n.Categories, tag)
                .Include(1);
        var newsEntry = await _client.GetEntries(newsBuilder);

        if (newsEntry != null && newsEntry.Any())
        {
            type = "C";
        }
        else
        {
            newsBuilder =
                new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                    .FieldMatches(n => n.Tags, tag)
                    .Include(1);
            newsEntry = await _client.GetEntries(newsBuilder);

            if (newsEntry != null && newsEntry.Any())
            {
                type = "T";
            }
        }

        if (newsEntry != null && newsEntry.Any())
        {
            var now = DateTime.Now.AddMinutes(5);
            var article = newsEntry.Where(e => now > e.SunriseDate)
                                    .Where(e => now < e.SunsetDate)
                                    .OrderByDescending(n => n.SunriseDate)
                                    .Take(1)
                                    .FirstOrDefault();

            if (article != null)
            {
                var newsArticle = _newsFactory.ToModel(article);
                result = newsArticle;
            }
        }

        return new ShowcaseNews() { News = result, Type = type };
    }
}