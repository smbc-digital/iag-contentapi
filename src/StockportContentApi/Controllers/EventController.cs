using System;
using System.ComponentModel;
using System.Threading.Tasks;
using Contentful.Core.Errors;
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

        [HttpGet]
        [Route("/api/{businessId}/events/{slug}")]
        public async Task<IActionResult> Detail(string slug, string businessId, [FromQuery] DateTime? date)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.GetEvent(slug, date);
            });
        }

        [HttpGet]
        [Route("/api/{businessId}/events")]
        [Route("/api/{businessId}/events/latest/{limit}")]
        public async Task<IActionResult> Index(string businessId, int limit = 0, [FromQuery] DateTime? dateFrom = null, [FromQuery] DateTime? dateTo = null, [FromQuery] string category = null)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.Get(dateFrom, dateTo, category, limit);
            });
        }

    }
}
