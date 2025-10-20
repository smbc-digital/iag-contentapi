namespace StockportContentApi.Repositories;

public interface IStartPageRepository
{
    Task<HttpResponse> GetStartPage(string startPageSlug, string tagId);
    Task<HttpResponse> Get(string tagId);
}

public class StartPageRepository(ContentfulConfig config,
                                IContentfulClientManager contentfulClientManager,
                                IContentfulFactory<ContentfulStartPage, StartPage> contentfulFactory,
                                ITimeProvider timeProvider) : IStartPageRepository
{
    private readonly IContentfulFactory<ContentfulStartPage, StartPage> _contentfulFactory = contentfulFactory;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly DateComparer _dateComparer = new(timeProvider);

    public async Task<HttpResponse> GetStartPage(string startPageSlug, string tagId)
    {
        QueryBuilder<ContentfulStartPage> builder = new QueryBuilder<ContentfulStartPage>()
            .ContentTypeIs("startPage")
            .FieldEquals("fields.slug", startPageSlug)
            .Include(3);
        
        ContentfulCollection<ContentfulStartPage> entries = await _client.GetEntries(builder);

        if (!entries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'");

        ContentfulStartPage startPageEntry = entries.FirstOrDefault();
        StartPage startPage = _contentfulFactory.ToModel(startPageEntry);

        if (!_dateComparer.DateNowIsWithinSunriseAndSunsetDates(startPageEntry.SunriseDate, startPageEntry.SunsetDate))
            startPage = new NullStartPage();

        return startPage.GetType().Equals(typeof(NullStartPage))
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'")
            : HttpResponse.Successful(startPage);
    }

    public async Task<HttpResponse> Get(string tagId)
    {
        QueryBuilder<ContentfulStartPage> builder = new QueryBuilder<ContentfulStartPage>()
            .ContentTypeIs("startPage")
            .Include(3);
        
        ContentfulCollection<ContentfulStartPage> entries = await _client.GetEntries(builder);

        if (!entries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found");

        IEnumerable<StartPage> startPages = entries.Select(_contentfulFactory.ToModel);

        return startPages is null || !startPages.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No start page found")
            : HttpResponse.Successful(startPages);
    }
}