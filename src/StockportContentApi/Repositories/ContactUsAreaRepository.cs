namespace StockportContentApi.Repositories;

public interface IContactUsAreaRepository
{
    Task<HttpResponse> GetContactUsArea(string tagId);
}

public class ContactUsAreaRepository(ContentfulConfig config,
                                    IContentfulClientManager clientManager,
                                    IContentfulFactory<ContentfulContactUsArea, ContactUsArea> contentfulFactory,
                                    ICache cache,
                                    IConfiguration configuration) : IContactUsAreaRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulContactUsArea, ContactUsArea> _contentfulFactory = contentfulFactory;
    private readonly ICache _cache = cache;
    private readonly int _contactUsAreaTimeout = GetCacheConfiguration(configuration);

    public async Task<HttpResponse> GetContactUsArea(string tagId)
    {
        ContentfulContactUsArea entry = await _cache.GetFromCacheOrDirectlyWithLockingAsync(
            $"{config.BusinessId}-contact-us-area",
            GetContactUsAreaFromSource,
            _contactUsAreaTimeout);
        ContactUsArea contactUsArea = _contentfulFactory.ToModel(entry);

        if (contactUsArea is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No contact us area found");

        return HttpResponse.Successful(contactUsArea);
    }

    private async Task<ContentfulContactUsArea> GetContactUsAreaFromSource()
    {
        QueryBuilder<ContentfulContactUsArea> builder = new QueryBuilder<ContentfulContactUsArea>()
            .ContentTypeIs("contactUsArea")
            .Include(3);
        ContentfulCollection<ContentfulContactUsArea> entries = await _client.GetEntries(builder);

        return entries.FirstOrDefault();
    }

    private static int GetCacheConfiguration(IConfiguration configuration)
    {
        int.TryParse(configuration["redisExpiryTimes:ContactUsArea"], out int timeout);
        return timeout;
    }
}