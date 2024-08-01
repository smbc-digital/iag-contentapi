namespace StockportContentApi.Repositories;

public interface IProfileRepository
{
    Task<HttpResponse> GetProfile(string slug);
    Task<HttpResponse> Get();
}

public class ProfileRepository : IProfileRepository
{
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;

    public ProfileRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulProfile, Profile> profileFactory)
    {
        _client = clientManager.GetClient(config);
        _profileFactory = profileFactory;
    }

    public async Task<HttpResponse> GetProfile(string slug)
    {
        var builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(2);
        var entries = await _client.GetEntries(builder);
        var entry = entries.FirstOrDefault();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No profile found for '{slug}'")
            : HttpResponse.Successful(_profileFactory.ToModel(entry));
    }

    public async Task<HttpResponse> Get()
    {
        var builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").Include(1);
        var entries = await _client.GetEntries(builder);

        if (!entries.Any() || entries is null) 
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No profiles found");

        var models = entries.Select(_ => _profileFactory.ToModel(_));

        return HttpResponse.Successful(models);
    }
}