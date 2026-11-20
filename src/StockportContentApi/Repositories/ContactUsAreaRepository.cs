namespace StockportContentApi.Repositories;

public interface IContactUsAreaRepository
{
    Task<HttpResponse> GetContactUsArea(string tagId);
}

public class ContactUsAreaRepository(ContentfulConfig config,
                                    IContentfulClientManager clientManager,
                                    IContactUsAreaContentfulFactory contentfulFactory,
                                    ICache cache,
                                    IConfiguration configuration) : IContactUsAreaRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContactUsAreaContentfulFactory _contentfulFactory = contentfulFactory;
    private readonly ICache _cache = cache;
    private readonly int _contactUsAreaTimeout = GetCacheConfiguration(configuration);

    public async Task<HttpResponse> GetContactUsArea(string tagId)
    {
        ContactUsArea contactUsArea = await _cache.GetFromCacheOrDirectlyWithLockingAsync(
            $"{config.BusinessId}-contact-us-area-mapped",
            GetContactUsAreaFromSource,
            _contactUsAreaTimeout);

        if (contactUsArea is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No contact us area found");

        return HttpResponse.Successful(_contentfulFactory.FilterForCurrentDate(contactUsArea));
    }

    private async Task<ContactUsArea> GetContactUsAreaFromSource()
    {
        QueryBuilder<ContentfulContactUsArea> builder = new QueryBuilder<ContentfulContactUsArea>()
            .ContentTypeIs("contactUsArea")
            .Include(3);
        ContentfulCollection<ContentfulContactUsArea> entries = await _client.GetEntries(builder);
        ContentfulContactUsArea entry = entries.FirstOrDefault();

        return entry is null
            ? null
            : _contentfulFactory.ToCacheModel(entry);
    }

    private static int GetCacheConfiguration(IConfiguration configuration)
    {
        int.TryParse(configuration["redisExpiryTimes:ContactUsArea"], out int timeout);
        return timeout;
    }
}