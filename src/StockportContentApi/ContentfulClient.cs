using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using StockportContentApi.Http;

namespace StockportContentApi
{
    public interface IContentfulClient
    {
        Task<ContentfulResponse> Get(string url);
    }

    public class ContentfulClient : IContentfulClient
    {
        private readonly IHttpClient _httpClient;

        public ContentfulClient(IHttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ContentfulResponse> Get(string url)
        {
            var response = await _httpClient.Get(url);

            if (response == null || response.StatusCode != HttpStatusCode.OK) return new NullContentfulResponse();

            var content = JsonConvert.DeserializeObject<dynamic>(response.Get<string>());
            return new ContentfulResponse(content);
        }
    }
}