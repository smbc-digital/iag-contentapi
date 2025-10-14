namespace StockportContentApi.Repositories;

public interface IArticleRepository
{
    Task<HttpResponse> Get(string tagId);
    Task<HttpResponse> GetArticle(string articleSlug, string tagId);
}

public class ArticleRepository(ContentfulConfig config,
                            IContentfulClientManager contentfulClientManager,
                            ITimeProvider timeProvider,
                            IContentfulFactory<ContentfulArticle, Article> contentfulFactory,
                            IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> contentfulFactoryArticle,
                            IVideoRepository videoRepository,
                            EventRepository eventRepository,
                            ICache cache,
                            IOptions<RedisExpiryConfiguration> redisExpiryConfiguration) : BaseRepository, IArticleRepository
{
    private readonly IContentfulFactory<ContentfulArticle, Article> _contentfulFactory = contentfulFactory;
    private readonly IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> _contentfulFactoryArticle = contentfulFactoryArticle;
    private readonly DateComparer _dateComparer = new(timeProvider);
    private readonly EventRepository _eventRepository = eventRepository;
    private readonly ICache _cache = cache;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly IVideoRepository _videoRepository = videoRepository;
    private readonly RedisExpiryConfiguration _redisExpiryConfiguration = redisExpiryConfiguration.Value;

    public async Task<HttpResponse> Get(string tagId)
    {
        IEnumerable<ArticleSiteMap> articles = await GetArticlesFromContentful(tagId);

        if (articles is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No articles found");

        IEnumerable<ArticleSiteMap> entries = articles.Where(article => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(article.SunriseDate, article.SunsetDate));

        return entries.Any()
            ? HttpResponse.Successful(entries)
            : HttpResponse.Failure(HttpStatusCode.NotFound, "No articles found within sunrise and sunset dates");
    }

    public async Task<HttpResponse> GetArticle(string articleSlug, string tagId)
    {
        Article article = await GetArticleFromCacheOrContentful(articleSlug, tagId);

        if (article is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No article found with slug '{articleSlug}' for '{tagId}'");

        await GetArticleRelatedEvents(article, tagId);

        ProcessArticleContent(article);

        return HttpResponse.Successful(article);
    }

    private async Task GetArticleRelatedEvents(Article article, string tagId)
    {
        if (!string.IsNullOrEmpty(article.AssociatedTagCategory))
        {
            List<string> associatedTagsCategories = article.AssociatedTagCategory.Split(",").ToList();
            List<Event> events = new();

            foreach (string associatedTagCategory in associatedTagsCategories)
            {
                List<Event> categoryEvents = await _eventRepository.GetEventsByCategory(associatedTagCategory, true, tagId);
                if (categoryEvents is not null && categoryEvents.Any())
                    events.AddRange(categoryEvents);
                else
                {
                    List<Event> tagEvents = await _eventRepository.GetEventsByTag(associatedTagCategory, true, tagId);
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
    }

    private async Task<IEnumerable<ArticleSiteMap>> GetArticlesFromContentful(string tagId)
    {
        QueryBuilder<ContentfulArticleForSiteMap> builder = new QueryBuilder<ContentfulArticleForSiteMap>()
            .ContentTypeIs("article")
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(2);
        
        ContentfulCollection<ContentfulArticleForSiteMap> entries = await GetAllEntriesAsync(_client, builder);

        return entries?.Select(_contentfulFactoryArticle.ToModel);
    }

    private async Task<Article> GetArticleFromCacheOrContentful(string articleSlug, string tagId)
    {
        ContentfulArticle entry = await _cache.GetFromCacheOrDirectlyAsync($"article-{articleSlug}", () => GetArticleEntry(articleSlug, tagId), _redisExpiryConfiguration.Articles);

        if (entry is null)
            return null;

        // Query tagged profiles
        var profileQuery = new QueryBuilder<ContentfulProfile>()
            .ContentTypeIs("profile")
            .FieldEquals("metadata.tags.sys.id[in]", tagId);

        var taggedProfiles = await _client.GetEntries(profileQuery); // call contentful AGAIN!
        var taggedProfileIds = taggedProfiles.Select(profile => profile.Slug);

        // Filter profiles before mapping
        entry.Profiles = entry.Profiles
            .Where(profile => taggedProfileIds.Contains(profile.Slug))
            .ToList();

        Article article = _contentfulFactory.ToModel(entry);

        var contentfulEntry = new
        {
            sys = new
            {
                id = entry.Sys.Id,
                type = entry.Sys.Type,
                locale = entry.Sys.Locale,
                contentType = entry.Sys.ContentType,
                environment = entry.Sys.Environment,
                space = entry.Sys.Space
            },
            fields = new
            {
                title = entry.Title,
                body = entry.Body,
                image = entry.Image,
                alerts = entry.Alerts,
                alertsInline = entry.AlertsInline,
                documents = entry.Documents,
                inlineQuotes = entry.InlineQuotes,
                logoAreaTitle = entry.LogoAreaTitle,
                trustedLogos = entry.TrustedLogos,
                hideLastUpdated = entry.HideLastUpdated,
                metadata = entry.Metadata
            }
        };

        article.RawContentful = JObject.FromObject(contentfulEntry);

        return article;
    }

    private async Task<ContentfulArticle> GetArticleEntry(string articleSlug, string tagId=null)
    {
        QueryBuilder<ContentfulArticle> builder = new QueryBuilder<ContentfulArticle>()
            .ContentTypeIs("article")
            .FieldEquals("fields.slug", articleSlug)
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(3);

        ContentfulCollection<ContentfulArticle> entries = await _client.GetEntries(builder);

        //var rawEntry = await _client.GetEntriesRaw();

        return entries.FirstOrDefault();
    }

    private void ProcessArticleContent(Article article)
    {
        article.Body = _videoRepository.Process(article.Body);
        
        foreach (Section section in article.Sections.Where(section => section is not null))
        {
            section.Body = _videoRepository.Process(section.Body);
        }
    }
}