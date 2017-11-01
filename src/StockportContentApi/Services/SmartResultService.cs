using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StockportContentApi.Builders;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.Services
{
    public interface ISmartResultService
    {
        Task<SmartResult> GetSmartResultBySlug(string businessId, string slug);
    }

    public class SmartResultService : ISmartResultService
    {

        private readonly Func<ContentfulConfig, ISmartResultRepository> _smartResultRepository;
        private readonly IContentfulFactory<ContentfulSmartResult, SmartResult> _smartResultFactory;
        private readonly IContentfulConfigBuilder _contentfulConfigBuilder;

        public SmartResultService (Func<ContentfulConfig, ISmartResultRepository> smartResultRepository, IContentfulFactory<ContentfulSmartResult, SmartResult> smartResultFactory, IContentfulConfigBuilder contentfulConfigBuilder)
        {
            _smartResultFactory = smartResultFactory;
            _smartResultRepository = smartResultRepository;
            _contentfulConfigBuilder = contentfulConfigBuilder;
        }

        public async Task<SmartResult> GetSmartResultBySlug(string businessId, string slug)
        {
            var config = _contentfulConfigBuilder.Build(businessId);
            var repository = _smartResultRepository(config);

            var smartResult = await repository.Get(slug);

            return smartResult == null
                ? null
                : _smartResultFactory.ToModel(smartResult);
        }


    }
}
