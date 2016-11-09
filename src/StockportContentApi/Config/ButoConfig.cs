namespace StockportContentApi.Config
{
    public class ButoConfig
    {
        public ButoConfig(string baseUrl)
        {
            Ensure.ArgumentNotNullOrEmpty(baseUrl, "BUTO_BASEURL");

            BaseUrl = baseUrl;
        }

        public string BaseUrl { get; set; }
    }
}