namespace StockportContentApi.Services.Profile
{
    public interface IProfileService
    {
        Task<StockportContentApi.Model.Profile> GetProfile(string slug, string businessId);

        Task<List<StockportContentApi.Model.Profile>> GetProfiles(string businessId);
    }
}