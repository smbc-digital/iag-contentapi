using System;
using System.Threading.Tasks;
using Contentful.Core.Models;
using StockportContentApi.Builders;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.Services
{
     public interface IDocumentService
     {
         Task<Document> GetDocumentByAssetId(string businessId, string assetId);
         Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug);
     }

    public class DocumentsService : IDocumentService
    {
        private readonly IContentfulConfigBuilder _contentfulConfigBuilder;
        private readonly Func<ContentfulConfig, IDocumentRepository> _documentRepository;
        private readonly Func<ContentfulConfig, IGroupAdvisorRepository> _groupAdvisorRepository;
        private readonly Func<ContentfulConfig, IGroupRepository> _groupRepository;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;
        private readonly ILoggedInHelper _loggedInHelper;

        public DocumentsService(Func<ContentfulConfig, IDocumentRepository> documentRepository, Func<ContentfulConfig, IGroupAdvisorRepository> groupAdvisorRepository, Func<ContentfulConfig, IGroupRepository> groupRepository, IContentfulFactory<Asset, Document> documentFactory, IContentfulConfigBuilder contentfulConfigBuilder, ILoggedInHelper loggedInHelper)
        {
            _documentRepository = documentRepository;
            _groupAdvisorRepository = groupAdvisorRepository;
            _groupRepository = groupRepository;
            _documentFactory = documentFactory;
            _contentfulConfigBuilder = contentfulConfigBuilder;
            _loggedInHelper = loggedInHelper;
        }

        public async Task<Document> GetDocumentByAssetId(string businessId, string assetId)
        {
            var repository = _documentRepository(_contentfulConfigBuilder.Build(businessId));
            var assetResponse = await repository.Get(assetId);

            return assetResponse == null
                ? null 
                : _documentFactory.ToModel(assetResponse);
        }

        public async Task<Document> GetSecureDocumentByAssetId(string businessId, string assetId, string groupSlug)
        {
            var config = _contentfulConfigBuilder.Build(businessId);

            // get logged in person
            var user = _loggedInHelper.GetLoggedInPerson();

            // user isn't logged in
            if (string.IsNullOrEmpty(user.Email)) return null;

            // check if the user has access to the group
            var groupAdvisorsRepository = _groupAdvisorRepository(config);
            var groupAdvisorResponse = await groupAdvisorsRepository.CheckIfUserHasAccessToGroupBySlug(groupSlug, user.Email);

            // get asset
            var repository = _documentRepository(config);
            var assetResponse = await repository.Get(assetId);

            // get the group and check if the asset exists in the group
            var groupRepository = _groupRepository(config);
            var groupResponse = await groupRepository.GetGroup(groupSlug, false);

            var group = groupResponse.Get<Group>();

            // check if asset exists in the group
            var assetExistsInGroup = group.AdditionalDocuments.Exists(o => o.AssetId == assetResponse.SystemProperties.Id);

            return assetResponse == null || !groupAdvisorResponse || !assetExistsInGroup
                ? null
                : _documentFactory.ToModel(assetResponse);
        }
    }
}
