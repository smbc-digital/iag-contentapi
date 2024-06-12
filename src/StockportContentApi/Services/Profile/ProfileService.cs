namespace StockportContentApi.Services.Profile;

public class ProfileService : IProfileService
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, IProfileRepository> _createRepository;

    public ProfileService(Func<string, ContentfulConfig> createConfig, Func<ContentfulConfig, IProfileRepository> createRepository)
    {
        _createRepository = createRepository;
        _createConfig = createConfig;
    }

    public async Task<Model.Profile> GetProfile(string slug, string businessId)
    {
        IProfileRepository profileRepository = _createRepository(_createConfig(businessId));
        HttpResponse response = await profileRepository.GetProfile(slug);

        if (response.StatusCode.Equals(HttpStatusCode.OK))
            return response.Get<Model.Profile>();

        return null;
    }

    public async Task<List<Model.Profile>> GetProfiles(string businessId)
    {
        IProfileRepository profileRepository = _createRepository(_createConfig(businessId));
        HttpResponse response = await profileRepository.Get();

        if (response.StatusCode.Equals(HttpStatusCode.OK))
        {
            IEnumerable<Model.Profile> profiles = response.Get<IEnumerable<Model.Profile>>();
            return profiles.ToList();
        }

        return null;
    }
}