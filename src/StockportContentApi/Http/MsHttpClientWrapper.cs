namespace StockportContentApi.Http
{
    public interface IMsHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string url);
    }

    public class MsHttpClientWrapper : IMsHttpClientWrapper
    {
        private readonly System.Net.Http.HttpClient _client = new System.Net.Http.HttpClient();

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return _client.GetAsync(url);
        }
    }
}
