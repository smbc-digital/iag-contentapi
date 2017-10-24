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
     }

    public class DocumentsService : IDocumentService
    {
        private readonly IContentfulConfigBuilder _contentfulConfigBuilder;
        private readonly Func<ContentfulConfig, IDocumentRepository> _documentRepository;
        private readonly IContentfulFactory<Asset, Document> _documentFactory;

        public DocumentsService(Func<ContentfulConfig, IDocumentRepository> documentRepository, IContentfulFactory<Asset, Document> documentFactory, IContentfulConfigBuilder contentfulConfigBuilder)
        {
            _documentRepository = documentRepository;
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
    }
}
