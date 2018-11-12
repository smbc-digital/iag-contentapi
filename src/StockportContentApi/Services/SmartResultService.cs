using System;
using System.Threading.Tasks;
using StockportContentApi.Builders;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Exceptions;
using StockportContentApi.Model;
using StockportContentApi.Repositories;

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
                ? throw new ServiceException()
                : _smartResultFactory.ToModel(smartResult);
        }


    }
}
