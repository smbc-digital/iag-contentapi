namespace StockportContentApi.Repositories;

public interface IAssetRepository
{
    Task<Asset> Get(string assetId);
}

public class AssetRepository(ContentfulConfig config,
                            IContentfulClientManager contentfulClientManager,
                            ILogger<AssetRepository> logger) : IAssetRepository
{
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly ILogger<AssetRepository> _logger = logger;

    public async Task<Asset> Get(string assetId)
    {
        try
        {
            return await _client.GetAsset(assetId, QueryBuilder<Asset>.New);
        }
        catch (ContentfulException ex)
        {
            _logger.LogWarning(new EventId(), ex, $"There was a problem with getting assetId: {assetId} from contentful");
            return null;
        }
    }
}