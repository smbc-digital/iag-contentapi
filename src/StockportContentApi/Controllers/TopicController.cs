using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class TopicController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, TopicRepository> _createRepository;

        public TopicController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, TopicRepository> createRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _createRepository = createRepository;
        }

        [HttpGet]
        [Route("{businessId}/topics/{topicSlug}")]
        [Route("v1/{businessId}/topics/{topicSlug}")]
        public async Task<IActionResult> GetTopicByTopicSlug(string businessId, string topicSlug)
        {
            return await _handler.Get(() =>
            {
                var topicRepository = _createRepository(_createConfig(businessId));
                return topicRepository.GetTopicByTopicSlug(topicSlug);
            });
        }

        [HttpGet]
        [Route("{businessId}/topics/")]
        [Route("v1/{businessId}/topics/")]
        public async Task<IActionResult> Get(string businessId)
        {
            return await _handler.Get(() =>
            {
                var topicRepository = _createRepository(_createConfig(businessId));
                return topicRepository.Get();
            });
        }
    }
}