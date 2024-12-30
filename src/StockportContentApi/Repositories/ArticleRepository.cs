﻿namespace StockportContentApi.Repositories;

public class ArticleRepository : BaseRepository
{
    private readonly IContentfulFactory<ContentfulArticle, Article> _contentfulFactory;
    private readonly IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> _contentfulFactoryArticle;
    private readonly DateComparer _dateComparer;
    private readonly EventRepository _eventRepository;
    private readonly ICache _cache;
    private readonly IContentfulClient _client;
    private readonly IVideoRepository _videoRepository;
    private readonly RedisExpiryConfiguration _redisExpiryConfiguration;

    public ArticleRepository(ContentfulConfig config,
        IContentfulClientManager contentfulClientManager,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulArticle, Article> contentfulFactory,
        IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> contentfulFactoryArticle,
        IVideoRepository videoRepository,
        EventRepository eventRepository,
        ICache cache,
        IOptions<RedisExpiryConfiguration> redisExpiryConfiguration)
    {
        _contentfulFactory = contentfulFactory;
        _contentfulFactoryArticle = contentfulFactoryArticle;
        _videoRepository = videoRepository;
        _eventRepository = eventRepository;
        _cache = cache;
        _redisExpiryConfiguration = redisExpiryConfiguration.Value;
        _dateComparer = new DateComparer(timeProvider);
        _client = contentfulClientManager.GetClient(config);
    }

    public async Task<HttpResponse> Get()
    {
        IEnumerable<ArticleSiteMap> articles = await GetArticlesFromContentful();

        if (articles is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No articles found");

        IEnumerable<ArticleSiteMap> entries = articles.Where(article => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(article.SunriseDate, article.SunsetDate));

        return entries.Any()
            ? HttpResponse.Successful(entries)
            : HttpResponse.Failure(HttpStatusCode.NotFound, "No articles found within sunrise and sunset dates");
    }

    public async Task<HttpResponse> GetArticle(string articleSlug)
    {
        Article article = await GetArticleFromCacheOrContentful(articleSlug);

        if (article is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No article found for '{articleSlug}'");

        if (!string.IsNullOrEmpty(article.AssociatedTagCategory))
        {
            List<string> associatedTagsCategories = article.AssociatedTagCategory.Split(",").ToList();
            List<Event> events = new();

            foreach (string associatedTagCategory in associatedTagsCategories)
            {
                List<Event> categoryEvents = await _eventRepository.GetEventsByCategory(associatedTagCategory, true);
                if (categoryEvents is not null && categoryEvents.Any())
                    events.AddRange(categoryEvents);
                else
                {
                    List<Event> tagEvents = await _eventRepository.GetEventsByTag(associatedTagCategory, true);
                    if (tagEvents is not null)
                        events.AddRange(tagEvents);
                }
            }

            article.Events = events
                            .GroupBy(evnt => evnt.Slug)
                            .Select(evnt => evnt.First())
                            .OrderBy(evnt => evnt.EventDate)
                            .ThenBy(evnt => TimeSpan.Parse(evnt.StartTime))
                            .ThenBy(evnt => evnt.Title)
                            .Take(3).ToList();
        }

        ProcessArticleContent(article);

        return HttpResponse.Successful(article);
    }

    private async Task<IEnumerable<ArticleSiteMap>> GetArticlesFromContentful()
    {
        QueryBuilder<ContentfulArticleForSiteMap> builder = new QueryBuilder<ContentfulArticleForSiteMap>().ContentTypeIs("article").Include(2);
        ContentfulCollection<ContentfulArticleForSiteMap> entries = await GetAllEntriesAsync(_client, builder);

        return entries?.Select(_contentfulFactoryArticle.ToModel);
    }

    private async Task<Article> GetArticleFromCacheOrContentful(string articleSlug)
    {
        ContentfulArticle entry = await _cache.GetFromCacheOrDirectlyAsync($"article-{articleSlug}", () => GetArticleEntry(articleSlug), _redisExpiryConfiguration.Articles);

        if (entry is null)
            return null;

        return _contentfulFactory.ToModel(entry);
    }

    private async Task<ContentfulArticle> GetArticleEntry(string articleSlug)
    {
        QueryBuilder<ContentfulArticle> builder = new QueryBuilder<ContentfulArticle>()
            .ContentTypeIs("article")
            .FieldEquals("fields.slug", articleSlug)
            .Include(3);

        ContentfulCollection<ContentfulArticle> entries = await _client.GetEntries(builder);

        return entries.FirstOrDefault();
    }

    private void ProcessArticleContent(Article article)
    {
        article.Body = _videoRepository.Process(article.Body);
        
        foreach (Section section in article.Sections)
        {
            if (section is not null)
                section.Body = _videoRepository.Process(section.Body);
        }
    }
}