namespace StockportContentApi.Builders;

public interface IContentfulConfigBuilder
{
    ContentfulConfig Build(string businessId);
}

public class ContentfulConfigBuilder : IContentfulConfigBuilder
{
    private readonly IConfiguration _configuration;

    public ContentfulConfigBuilder(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public ContentfulConfig Build(string businessId)
    {
        return new ContentfulConfig(
            _configuration[$"{businessId}:Space"],
            _configuration[$"{businessId}:AccessKey"],
            _configuration[$"{businessId}:ManagementKey"]);
    }
}
