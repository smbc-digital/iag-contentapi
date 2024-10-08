namespace StockportContentApi.Config;

[ExcludeFromCodeCoverage]
public class TwentyThreeConfig
{
    public TwentyThreeConfig(string baseUrl)
    {
        Utils.Ensure.ArgumentNotNullOrEmpty(baseUrl, "TWENTY_THREE_BASEURL");

        BaseUrl = baseUrl;
    }

    public string BaseUrl { get; set; }
}
