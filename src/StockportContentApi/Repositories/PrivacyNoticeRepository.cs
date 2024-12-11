namespace StockportContentApi.Repositories;

public interface IPrivacyNoticeRepository
{
    Task<HttpResponse> GetPrivacyNotice(string slug);
    Task<HttpResponse> GetAllPrivacyNotices();
    Task<List<PrivacyNotice>> GetPrivacyNoticesByTitle(string title);
}

public class PrivacyNoticeRepository : IPrivacyNoticeRepository
{
    private readonly IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice> _contentfulFactory;
    private readonly IContentfulClient _client;

    public PrivacyNoticeRepository(ContentfulConfig config, IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice> contentfulFactory,
        IContentfulClientManager contentfulClientManager)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
    }

    public async Task<HttpResponse> GetPrivacyNotice(string slug)
    {
        QueryBuilder<ContentfulPrivacyNotice> builder = new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").FieldEquals("fields.slug", slug).Include(2);
        ContentfulCollection<ContentfulPrivacyNotice> entries = await _client.GetEntries(builder);
        ContentfulPrivacyNotice entry = entries.FirstOrDefault();

        PrivacyNotice privacyNotice = entry is null
            ? null
            : _contentfulFactory.ToModel(entry);

        return privacyNotice is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No privacy notice found for '{slug}'")
            : HttpResponse.Successful(privacyNotice);
    }

    public async Task<HttpResponse> GetAllPrivacyNotices()
    {
        QueryBuilder<ContentfulPrivacyNotice> builder = new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").Include(2);
        IEnumerable<ContentfulPrivacyNotice> entries = await GetAllEntries(builder);
        
        List<PrivacyNotice> privacyNotices = !entries.Any() || entries is null
            ? null
            : entries.Select(_contentfulFactory.ToModel).ToList();

        return !privacyNotices.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No privacy notices found")
            : HttpResponse.Successful(privacyNotices);
    }

    public async Task<List<PrivacyNotice>> GetPrivacyNoticesByTitle(string title)
    {
        QueryBuilder<ContentfulPrivacyNotice> builder = new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").Include(2);
        IEnumerable<ContentfulPrivacyNotice> entries = await GetAllEntries(builder);
        List<PrivacyNotice> convertedEntries = entries.Select(entry => _contentfulFactory.ToModel(entry)).ToList();

        return convertedEntries;
    }

    private async Task<IEnumerable<T>> GetAllEntries<T>(QueryBuilder<T> builder)
    {
        ContentfulCollection<T> entries = await _client.GetEntries(builder.Limit(ContentfulQueryValues.LIMIT_MAX));
        
        return entries.Items;
    }
}