namespace StockportContentApi.Repositories;

public interface IPrivacyNoticeRepository
{
    Task<PrivacyNotice> GetPrivacyNotice(string slug);
    Task<List<PrivacyNotice>> GetAllPrivacyNotices();
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

    public async Task<PrivacyNotice> GetPrivacyNotice(string slug)
    {
        QueryBuilder<ContentfulPrivacyNotice> builder = new QueryBuilder<ContentfulPrivacyNotice>()
            .ContentTypeIs("privacyNotice")
            .FieldEquals("fields.slug", slug)
            .Include(2);

        ContentfulCollection<ContentfulPrivacyNotice> entries = await _client.GetEntries(builder);
        ContentfulPrivacyNotice entry = entries.FirstOrDefault();

        return entry is not null
            ? _contentfulFactory.ToModel(entry)
            : null;
    }

    public async Task<List<PrivacyNotice>> GetAllPrivacyNotices()
    {
        QueryBuilder<ContentfulPrivacyNotice> builder = new QueryBuilder<ContentfulPrivacyNotice>()
            .ContentTypeIs("privacyNotice")
            .Include(2);

        IEnumerable<ContentfulPrivacyNotice> entries = await GetAllEntries(builder);

        List<PrivacyNotice> convertedEntries = entries.Select(entry => _contentfulFactory.ToModel(entry)).ToList();

        return convertedEntries;
    }

    public async Task<List<PrivacyNotice>> GetPrivacyNoticesByTitle(string title)
    {
        QueryBuilder<ContentfulPrivacyNotice> builder = new QueryBuilder<ContentfulPrivacyNotice>()
            .ContentTypeIs("privacyNotice")
            .Include(2);

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