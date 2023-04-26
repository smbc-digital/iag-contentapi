namespace StockportContentApi.Services.Profile;

public interface IProfileService
{
    Task<Model.Profile> GetProfile(string slug, string businessId);

    Task<List<Model.Profile>> GetProfiles(string businessId);
}