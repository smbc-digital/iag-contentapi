using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contentful.Core.Models;
using StockportContentApi.Builders;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.Model;
using StockportContentApi.Repositories;

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
        private readonly IContentfulFactory<Asset, Document> _documentFactory;

        public DocumentsService(Func<ContentfulConfig, IDocumentRepository> documentRepository, Func<ContentfulConfig, IGroupAdvisorRepository> groupAdvisorRepository, IContentfulFactory<Asset, Document> documentFactory, IContentfulConfigBuilder contentfulConfigBuilder)
        {
            _documentRepository = documentRepository;
            _groupAdvisorRepository = groupAdvisorRepository;
            _documentFactory = documentFactory;
            _contentfulConfigBuilder = contentfulConfigBuilder;
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
            var repository = _documentRepository(_contentfulConfigBuilder.Build(businessId));
            var assetResponse = await repository.Get(assetId);

            // TODO: Get email from cookie and jwt decode it
            var email = "";

            var groupAdvisorsRepository = _groupAdvisorRepository(_contentfulConfigBuilder.Build(businessId));
            var groupAdvisorResponse = await groupAdvisorsRepository.CheckIfUserHasAccessToGroupBySlug(groupSlug, email);

            return assetResponse == null || !groupAdvisorResponse
                ? null
                : _documentFactory.ToModel(assetResponse);
        }
    }
}
