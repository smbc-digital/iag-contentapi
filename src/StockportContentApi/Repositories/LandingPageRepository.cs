namespace StockportContentApi.Repositories;

public class LandingPageRepository : BaseRepository
{
    private readonly IContentfulFactory<ContentfulLandingPage, LandingPage> _contentfulFactory;
    private readonly IContentfulClient _client;

    public LandingPageRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulLandingPage, LandingPage> contentfulFactory,
        IContentfulClientManager contentfulClientManager)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
    }

    public async Task<HttpResponse> GetLandingPage(string slug)
    {
        QueryBuilder<ContentfulLandingPage> builder = new QueryBuilder<ContentfulLandingPage>().ContentTypeIs("landingPage").FieldEquals("fields.slug", slug).Include(2);
        ContentfulCollection<ContentfulLandingPage> entries = await _client.GetEntries(builder);
        ContentfulLandingPage entry = entries.FirstOrDefault();
        
        if(entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No Landing Page found");
        
        LandingPage landingPage = _contentfulFactory.ToModel(entry);

        return landingPage is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Landing page not found {slug}")
            : HttpResponse.Successful(landingPage);  
    }
}