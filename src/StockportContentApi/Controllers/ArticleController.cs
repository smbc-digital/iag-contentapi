using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class ArticleController
    {
        
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, ArticleRepository> _createRepository;

        public ArticleController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, ArticleRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/article/{articleSlug}")]
        public async Task<IActionResult> GetArticle(string articleSlug,string  businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                var article = repository.GetArticle(articleSlug);

                return article;
            });
        }

        [HttpGet]
        [Route("api/{businessId}/articleSiteMap")]
        public async Task<IActionResult> Index(string businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _createRepository(_createConfig(businessId));
                return repository.Get();
            });
        }
    }
}