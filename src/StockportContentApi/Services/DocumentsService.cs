namespace StockportContentApi.Services;

public interface IDocumentService
{
    Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug);
}

public class DocumentsService(Func<ContentfulConfig, IAssetRepository> documentRepository,
                            IContentfulFactory<Asset, Document> documentFactory,
                            IContentfulConfigBuilder contentfulConfigBuilder) : IDocumentService
{
    private readonly IContentfulConfigBuilder _contentfulConfigBuilder = contentfulConfigBuilder;
    private readonly IContentfulFactory<Asset, Document> _documentFactory = documentFactory;
    private readonly Func<ContentfulConfig, IAssetRepository> _documentRepository = documentRepository;

    public async Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug)
    {
        ContentfulConfig config = _contentfulConfigBuilder.Build(businessId);

        Asset asset = await GetDocumentAsAsset(assetId, config);

        return asset is null
            ? null
            : _documentFactory.ToModel(asset);
    }

    private async Task<Asset> GetDocumentAsAsset(string assetId, ContentfulConfig config)
    {
        IAssetRepository repository = _documentRepository(config);
        Asset assetResponse = await repository.Get(assetId);

        return assetResponse;
    }
}