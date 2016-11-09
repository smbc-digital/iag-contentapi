using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class NewsController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, NewsRepository> _newsRepository;

        public NewsController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, NewsRepository> newsRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _newsRepository = newsRepository;
        }
        
        [Route("/api/{businessId}/news")]
        public async Task<IActionResult> Index(string businessId, [FromQuery] string tag = null, [FromQuery] string category = null)
        {
            return await _handler.Get(() =>
            {
                var repository = _newsRepository(_createConfig(businessId));
                return repository.Get(tag, category);
            });
        }

        [Route("/api/{businessId}/news/latest/{limit}")]
        public async Task<IActionResult> LatestNews(string businessId, int limit)
        {
            return await _handler.Get(() =>
            {
                var repository = _newsRepository(_createConfig(businessId));
                return repository.GetNewsByLimit(limit);
            });
        }

        [Route("/api/{businessId}/news/{slug}")]
        public async Task<IActionResult> Detail(string slug, string businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _newsRepository(_createConfig(businessId));
                return repository.GetNews(slug);
            });
        }
    }
}
