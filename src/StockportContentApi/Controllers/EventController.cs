using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class EventController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, EventRepository> _eventRepository;

        public EventController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, EventRepository> eventRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _eventRepository = eventRepository;
        }

        [Route("/api/{businessId}/events/{slug}")]
        public async Task<IActionResult> Detail(string slug, string businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.GetEvent(slug);
            });
        }

        [Route("/api/{businessId}/events")]
        public async Task<IActionResult> Index(string businessId,[FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.Get();
            });
        }
    }
}
