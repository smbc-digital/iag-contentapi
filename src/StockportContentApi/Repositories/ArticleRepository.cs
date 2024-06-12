namespace StockportContentApi.Repositories;

public class ArticleRepository : BaseRepository
{
    private readonly IContentfulFactory<ContentfulArticle, Article> _contentfulFactory;
    private readonly IContentfulFactory<ContentfulArticleForSiteMap, ArticleSiteMap> _contentfulFactoryArticle;
    private readonly DateComparer _dateComparer;
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
        ICache cache,
        IOptions<RedisExpiryConfiguration> redisExpiryConfiguration)
    {
        _contentfulFactory = contentfulFactory;
        _contentfulFactoryArticle = contentfulFactoryArticle;
        _videoRepository = videoRepository;
        _cache = cache;
        _redisExpiryConfiguration = redisExpiryConfiguration.Value;
        _dateComparer = new DateComparer(timeProvider);
        _client = contentfulClientManager.GetClient(config);
    }

    public async Task<HttpResponse> Get()
    {
        var articles = await GetArticlesFromContentful();
        
        if (articles is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No articles found");

        var entries = articles.Where(article => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(article.SunriseDate, article.SunsetDate));
        return entries.Any()
            ? HttpResponse.Successful(entries)
            : HttpResponse.Failure(HttpStatusCode.NotFound, "No articles found within sunrise and sunset dates");
    }

    public async Task<HttpResponse> GetArticle(string articleSlug)
    {
        var article = await GetArticleFromCacheOrContentful(articleSlug);

        if (article is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No article found for '{articleSlug}'");

        ProcessArticleContent(article);
        return HttpResponse.Successful(article);
    }

    private async Task<IEnumerable<ArticleSiteMap>> GetArticlesFromContentful()
    {
        var builder = new QueryBuilder<ContentfulArticleForSiteMap>().ContentTypeIs("article").Include(2);
        var entries = await GetAllEntriesAsync(_client, builder);
        return entries?.Select(entry => _contentfulFactoryArticle.ToModel(entry));
    }

    private async Task<Article> GetArticleFromCacheOrContentful(string articleSlug)
    {
        var entry = await _cache.GetFromCacheOrDirectlyAsync($"article-{articleSlug}", () => GetArticleEntry(articleSlug), _redisExpiryConfiguration.Articles);

        if (entry is null)
            return null;

        return _contentfulFactory.ToModel(entry);
    }

    private async Task<ContentfulArticle> GetArticleEntry(string articleSlug)
    {
        var builder = new QueryBuilder<ContentfulArticle>()
            .ContentTypeIs("article")
            .FieldEquals("fields.slug", articleSlug)
            .Include(3);

        var entries = await _client.GetEntries(builder);
        return entries.FirstOrDefault();
    }

    private void ProcessArticleContent(Article article)
    {
        article.Body = _videoRepository.Process(article.Body);
        foreach (var section in article.Sections)
        {
            if (section is not null)
                section.Body = _videoRepository.Process(section.Body);
        }
    }
}