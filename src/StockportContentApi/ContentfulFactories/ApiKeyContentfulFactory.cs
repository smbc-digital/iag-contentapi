using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class ApiKeyContentfulFactory : IContentfulFactory<ContentfulApiKey, ApiKey>
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ApiKeyContentfulFactory(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public ApiKey ToModel(ContentfulApiKey entryContentfulApiKey)
        {
            return new ApiKey(entryContentfulApiKey.Name, entryContentfulApiKey.Key, entryContentfulApiKey.Email,
                entryContentfulApiKey.ActiveFrom, entryContentfulApiKey.ActiveTo, entryContentfulApiKey.EndPoints, 
                entryContentfulApiKey.Version, entryContentfulApiKey.CanViewSensitive).StripData(_httpContextAccessor);
        }
    }
}