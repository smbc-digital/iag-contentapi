using Contentful.Core.Models;
using StockportContentApi.Builders;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;
using Document = StockportContentApi.Model.Document;

namespace StockportContentApi.Services
{
    public interface IDocumentService
    {
        Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug);
    }

    public class DocumentsService : IDocumentService
    {
        private readonly IContentfulConfigBuilder _contentfulConfigBuilder;
        private readonly Func<ContentfulConfig, IAssetRepository> _documentRepository;
        private readonly Func<ContentfulConfig, IGroupAdvisorRepository> _groupAdvisorRepository;
        private readonly Func<ContentfulConfig, IGroupRepository> _groupRepository;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly ILoggedInHelper _loggedInHelper;

        public DocumentsService(Func<ContentfulConfig, IAssetRepository> documentRepository, Func<ContentfulConfig, IGroupAdvisorRepository> groupAdvisorRepository, Func<ContentfulConfig, IGroupRepository> groupRepository, IContentfulFactory<Asset, Document> documentFactory, IContentfulConfigBuilder contentfulConfigBuilder, ILoggedInHelper loggedInHelper)
        {
            _documentRepository = documentRepository;
            _groupAdvisorRepository = groupAdvisorRepository;
            _groupRepository = groupRepository;
            _documentFactory = documentFactory;
            _contentfulConfigBuilder = contentfulConfigBuilder;
            _loggedInHelper = loggedInHelper;
        }

        public async Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug)
        {
            var config = _contentfulConfigBuilder.Build(businessId);
            var user = _loggedInHelper.GetLoggedInPerson();

            var hasPermission = await IsUserAdvisorForGroup(groupSlug, config, user);

            if (!hasPermission) return null;

            var asset = await GetDocumentAsAsset(assetId, config);

            return asset == null || !await DoesGroupReferenceAsset(groupSlug, config, asset)
                ? null
                : _documentFactory.ToModel(asset);
        }

        private async Task<bool> DoesGroupReferenceAsset(string groupSlug, ContentfulConfig config, Asset asset)
        {
            var group = await GetGroup(groupSlug, config);

            var assetExistsInGroup = group.AdditionalDocuments.Exists(o => o.AssetId == asset.SystemProperties.Id);
            return assetExistsInGroup;
        }

        private async Task<Group> GetGroup(string groupSlug, ContentfulConfig config)
        {
            var groupRepository = _groupRepository(config);
            var groupResponse = await groupRepository.GetGroup(groupSlug, false);
            return groupResponse.Get<Group>();
        }

        private async Task<Asset> GetDocumentAsAsset(string assetId, ContentfulConfig config)
        {
            var repository = _documentRepository(config);
            var assetResponse = await repository.Get(assetId);
            return assetResponse;
        }

        private async Task<bool> IsUserAdvisorForGroup(string groupSlug, ContentfulConfig config, LoggedInPerson user)
        {
            if (string.IsNullOrEmpty(user.Email)) return false;

            var groupAdvisorsRepository = _groupAdvisorRepository(config);
            var groupAdvisorResponse = await groupAdvisorsRepository.CheckIfUserHasAccessToGroupBySlug(groupSlug, user.Email);
            return groupAdvisorResponse;
        }
    }
}
