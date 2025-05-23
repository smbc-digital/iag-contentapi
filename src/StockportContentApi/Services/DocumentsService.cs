﻿namespace StockportContentApi.Services;

public interface IDocumentService
{
    Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug);
}

public class DocumentsService(Func<ContentfulConfig, IAssetRepository> documentRepository,
                            Func<ContentfulConfig, IGroupAdvisorRepository> groupAdvisorRepository,
                            Func<ContentfulConfig, IGroupRepository> groupRepository,
                            IContentfulFactory<Asset, Document> documentFactory,
                            IContentfulConfigBuilder contentfulConfigBuilder,
                            ILoggedInHelper loggedInHelper) : IDocumentService
{
    private readonly IContentfulConfigBuilder _contentfulConfigBuilder = contentfulConfigBuilder;
    private readonly IContentfulFactory<Asset, Document> _documentFactory = documentFactory;
    private readonly Func<ContentfulConfig, IAssetRepository> _documentRepository = documentRepository;
    private readonly Func<ContentfulConfig, IGroupAdvisorRepository> _groupAdvisorRepository = groupAdvisorRepository;
    private readonly Func<ContentfulConfig, IGroupRepository> _groupRepository = groupRepository;
    private readonly ILoggedInHelper _loggedInHelper = loggedInHelper;

    public async Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug)
    {
        ContentfulConfig config = _contentfulConfigBuilder.Build(businessId);
        LoggedInPerson user = _loggedInHelper.GetLoggedInPerson();
        bool hasPermission = await IsUserAdvisorForGroup(groupSlug, config, user);

        if (!hasPermission)
            return null;

        Asset asset = await GetDocumentAsAsset(assetId, config);

        return asset is null || !await DoesGroupReferenceAsset(groupSlug, config, asset)
            ? null
            : _documentFactory.ToModel(asset);
    }

    private async Task<bool> DoesGroupReferenceAsset(string groupSlug, ContentfulConfig config, Asset asset)
    {
        Group group = await GetGroup(groupSlug, config);
        bool assetExistsInGroup = group.AdditionalDocuments.Exists(_ => _.AssetId.Equals(asset.SystemProperties.Id));

        return assetExistsInGroup;
    }

    private async Task<Group> GetGroup(string groupSlug, ContentfulConfig config)
    {
        IGroupRepository groupRepository = _groupRepository(config);
        HttpResponse groupResponse = await groupRepository.GetGroup(groupSlug, false);

        return groupResponse.Get<Group>();
    }

    private async Task<Asset> GetDocumentAsAsset(string assetId, ContentfulConfig config)
    {
        IAssetRepository repository = _documentRepository(config);
        Asset assetResponse = await repository.Get(assetId);

        return assetResponse;
    }

    private async Task<bool> IsUserAdvisorForGroup(string groupSlug, ContentfulConfig config, LoggedInPerson user)
    {
        if (string.IsNullOrEmpty(user.Email))
            return false;

        IGroupAdvisorRepository groupAdvisorsRepository = _groupAdvisorRepository(config);
        bool groupAdvisorResponse = await groupAdvisorsRepository.CheckIfUserHasAccessToGroupBySlug(groupSlug, user.Email);

        return groupAdvisorResponse;
    }
}