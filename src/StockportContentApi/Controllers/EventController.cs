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

    }
}
