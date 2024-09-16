namespace StockportContentApi.Repositories;

public class LandingPageRepository : BaseRepository
{
    private readonly IContentfulFactory<ContentfulLandingPage, LandingPage> _contentfulFactory;
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulNews, News> _newsFactory;

    public LandingPageRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulLandingPage, LandingPage> contentfulFactory,
        IContentfulClientManager contentfulClientManager,
        IContentfulFactory<ContentfulNews, News> newsFactory)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
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

        foreach (ContentBlock contentBlock in landingPage.PageSections)
        {
            if (contentBlock.ContentType.Equals("NewsBanner") && !string.IsNullOrEmpty(contentBlock.AssociatedTagCategory))
            {
                ContentfulNews latestNewsResponse = await GetLatestNewsByTag(contentBlock.AssociatedTagCategory);

                if (latestNewsResponse is not null)
                    contentBlock.NewsArticle = _newsFactory.ToModel(latestNewsResponse);
            }
        }

        return landingPage is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Landing page not found {slug}")
            : HttpResponse.Successful(landingPage);  
    }

    private async Task<ContentfulNews> GetLatestNewsByTag(string tag)
    {
        QueryBuilder<ContentfulNews> newsBuilder = new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                .FieldMatches(n => n.Categories, tag)
                .Include(1);

        ContentfulCollection<ContentfulNews> newsEntries = await _client.GetEntries(newsBuilder);
        return newsEntries.FirstOrDefault();
    }
}