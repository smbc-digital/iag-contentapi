using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class TopicController
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, TopicRepository> _createRepository;

        public TopicController(ResponseHandler handler, 
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, TopicRepository>  createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/topic/{topicSlug}")]
        public async Task<IActionResult> GetTopicByTopicSlug(string businessId, string topicSlug)
        {
            return await _handler.Get(() =>
            {
                var topicRepository = _createRepository(_createConfig(businessId));
                return topicRepository.GetTopicByTopicSlug(topicSlug);
            });
        }
    }
}