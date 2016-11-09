using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class ProfileRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private const string ContentType = "profile";
        private readonly IFactory<Profile> _factory;

        public ProfileRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<Profile> factory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _factory = factory;
        }

        public async Task<HttpResponse> GetProfile(string profileSlug)
        {
            var referenceLevelLimit = 1;
            var contentfulResponse = await _contentfulClient.Get(UrlFor(ContentType, referenceLevelLimit, profileSlug));

            if (!contentfulResponse.HasItems()) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No profile found for '{profileSlug}'");

            var profile = _factory.Build(contentfulResponse.GetFirstItem() ,contentfulResponse);
            return HttpResponse.Successful(profile);
        }

        private string UrlFor(string type, int referenceLevel, string slug = null)
        {
            return slug == null
                ? $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}"
                : $"{_contentfulApiUrl}&content_type={type}&include={referenceLevel}&fields.slug={slug}";
        }
    }
}
