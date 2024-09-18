namespace StockportContentApi.Repositories;

public class LandingPageRepository : BaseRepository
{
    private readonly IContentfulFactory<ContentfulLandingPage, LandingPage> _contentfulFactory;
    private readonly IContentfulClient _client;
    private readonly EventRepository _eventRepository;
    private readonly IContentfulFactory<ContentfulNews, News> _newsFactory;

    public LandingPageRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulLandingPage, LandingPage> contentfulFactory,
        IContentfulClientManager contentfulClientManager,
        EventRepository eventRepository,
        IContentfulFactory<ContentfulNews, News> newsFactory)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
        _eventRepository = eventRepository;
        _newsFactory = newsFactory;
    }

    public async Task<HttpResponse> GetLandingPage(string slug)
    {
        QueryBuilder<ContentfulLandingPage> builder = new QueryBuilder<ContentfulLandingPage>().ContentTypeIs("landingPage").FieldEquals("fields.slug", slug).Include(2);
        ContentfulCollection<ContentfulLandingPage> entries = await _client.GetEntries(builder);
        ContentfulLandingPage entry = entries.FirstOrDefault();
        
        if(entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No Landing Page found");
        
        LandingPage landingPage = _contentfulFactory.ToModel(entry);

        if (landingPage is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"Landing page not found {slug}");

        if(landingPage.PageSections is not null && landingPage.PageSections.Any())
        {
            foreach (ContentBlock contentBlock in landingPage.PageSections.Where(contentBlock => !string.IsNullOrEmpty(contentBlock.ContentType) && contentBlock.ContentType.Equals("NewsBanner") && !string.IsNullOrEmpty(contentBlock.AssociatedTagCategory)))
            {
                ContentfulNews latestNewsResponse = await GetLatestNewsByTagOrCategory(contentBlock.AssociatedTagCategory);
                if (latestNewsResponse is not null)
                    contentBlock.NewsArticle = _newsFactory.ToModel(latestNewsResponse);
            }

            foreach (ContentBlock contentBlock in landingPage.PageSections.Where(contentBlock => !string.IsNullOrEmpty(contentBlock.ContentType) && contentBlock.ContentType.Equals("EventCards") && !string.IsNullOrEmpty(contentBlock.AssociatedTagCategory)))
            {
                List<Event> events = await _eventRepository.GetEventsByCategory(contentBlock.AssociatedTagCategory, true);

                if (!events.Any())
                    events = await _eventRepository.GetEventsByTag(contentBlock.AssociatedTagCategory, true);

                contentBlock.Events = events.Take(3).ToList();
            }        
        }
        
        return landingPage is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Landing page not found {slug}")
            : HttpResponse.Successful(landingPage);  
    }

    internal async Task<ContentfulNews> GetLatestNewsByTagOrCategory(string tag)
    {
        QueryBuilder<ContentfulNews> newsBuilderUsingCategory = new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                .FieldMatches(n => n.Categories, tag)
                .Include(1);

        ContentfulCollection<ContentfulNews> newsEntries = await _client.GetEntries(newsBuilderUsingCategory);

        if (newsEntries is null || !newsEntries.Items?.Any() is true)
        {
            QueryBuilder<ContentfulNews> newsBuilderUsingTag = new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                .FieldMatches(n => n.Tags, tag)
                .Include(1);

            newsEntries = await _client.GetEntries(newsBuilderUsingTag);
        }

        return newsEntries.FirstOrDefault();
    }
}