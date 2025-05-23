﻿namespace StockportContentApi.Repositories;

public interface IProfileRepository
{
    Task<HttpResponse> GetProfile(string slug);
    Task<HttpResponse> Get();
}

public class ProfileRepository(ContentfulConfig config,
                            IContentfulClientManager clientManager,
                            IContentfulFactory<ContentfulProfile, Profile> profileFactory) : IProfileRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory = profileFactory;

    public async Task<HttpResponse> GetProfile(string slug)
    {
        QueryBuilder<ContentfulProfile> builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(2);
        ContentfulCollection<ContentfulProfile> entries = await _client.GetEntries(builder);
        ContentfulProfile entry = entries.FirstOrDefault();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No profile found for '{slug}'")
            : HttpResponse.Successful(_profileFactory.ToModel(entry));
    }

    public async Task<HttpResponse> Get()
    {
        QueryBuilder<ContentfulProfile> builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").Include(1);
        ContentfulCollection<ContentfulProfile> entries = await _client.GetEntries(builder);

        if (!entries.Any() || entries is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No profiles found");

        IEnumerable<Profile> models = entries.Select(_profileFactory.ToModel);

        return HttpResponse.Successful(models);
    }
}