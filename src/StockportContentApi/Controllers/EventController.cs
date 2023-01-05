using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.ContentfulModels;
using StockportContentApi.ManagementModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;

namespace StockportContentApi.Controllers
{
    public class EventController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<string, ContentfulConfig> _createConfig;
        private readonly Func<ContentfulConfig, EventRepository> _eventRepository;
        private readonly Func<ContentfulConfig, EventCategoryRepository> _eventCategoryRepository;
        private readonly Func<ContentfulConfig, ManagementRepository> _managementRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<EventController> _logger;

        public EventController(
            ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, EventRepository> eventRepository,
            Func<ContentfulConfig, EventCategoryRepository> eventCategoryRepository,
            Func<ContentfulConfig, ManagementRepository> managementRepository,
            IMapper mapper,
            ILogger<EventController> logger)
        {
            _handler = handler;
            _createConfig = createConfig;
            _eventRepository = eventRepository;
            _managementRepository = managementRepository;
            _eventCategoryRepository = eventCategoryRepository;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        [Route("{businessId}/event-categories")]
        [Route("v1/{businessId}/event-categories")]
        public async Task<IActionResult> GetEventCategories(string businessId)
        {
            return await _handler.Get(() =>
            {
                var eventRepository = _eventCategoryRepository(_createConfig(businessId));
                return eventRepository.GetEventCategories();
            });
        }

        [HttpGet]
        [Route("{businessId}/eventhomepage")]
        [Route("v1/{businessId}/eventhomepage")]
        public async Task<IActionResult> Homepage(string businessId)
        {
            var categoryRepository = _eventCategoryRepository(_createConfig(businessId));
            var categoriesresponse = await categoryRepository.GetEventCategories();
            var categories = categoriesresponse.Get<List<EventCategory>>();

            var repository = _eventRepository(_createConfig(businessId));
            var response = await repository.GetEventHomepage();
            var homepage = response.Get<EventHomepage>();

            homepage.Categories = categories;
            return Ok(homepage);
        }

        [HttpGet]
        [Route("{businessId}/events/{slug}")]
        [Route("v1/{businessId}/events/{slug}")]
        public async Task<IActionResult> Detail(string slug, string businessId, [FromQuery] DateTime? date)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.GetEvent(slug, date);
            });
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPut]
        [Route("{businessId}/events/{slug}")]
        [Route("v1/{businessId}/events/{slug}")]
        public async Task<IActionResult> UpdateEvent([FromBody] Event eventDetail, string businessId)
        {
            var repository = _eventRepository(_createConfig(businessId));
            var existingEvent = await repository.GetContentfulEvent(eventDetail.Slug);

            var existingCategories = await repository.GetContentfulEventCategories();
            var referencedCategories = existingCategories.Items.Where(c => eventDetail.EventCategories.Any(ed => c.Name == ed.Name)).ToList();

            var managementEvent = ConvertToManagementEvent(eventDetail, referencedCategories, existingEvent);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                var version = await managementRepository.GetVersion(existingEvent.Sys.Id);
                existingEvent.Sys.Version = version;
                var result = await managementRepository.CreateOrUpdate(managementEvent, existingEvent.Sys);
                return result;
            });
        }

        [HttpGet]
        [Route("{businessId}/events")]
        [Route("{businessId}/events/latest/{limit}")]
        [Route("v1/{businessId}/events")]
        [Route("v1/{businessId}/events/latest/{limit}")]
        public async Task<IActionResult> Index(
            string businessId,
            int limit = 0,
            [FromQuery] DateTime? dateFrom = null,
            [FromQuery] DateTime? dateTo = null,
            [FromQuery] string category = null,
            [FromQuery] bool? featured = null,
            [FromQuery] string tag = null,
            [FromQuery] string price = null, [FromQuery] double latitude = 0, [FromQuery] double longitude = 0)
        {
            return await _handler.Get(() =>
            {
                var repository = _eventRepository(_createConfig(businessId));
                return repository.Get(dateFrom, dateTo, category, limit, featured, tag, price, latitude, longitude);
            });
        }

        [HttpGet]
        [Route("{businessId}/events/by-category")]
        [Route("v1/{businessId}/events/by-category")]
        public async Task<IActionResult> GetEventsByCatrgoryOrTag(string businessId, [FromQuery] string category = "", bool onlyNextOccurrence = true)
        {
            var repository = _eventRepository(_createConfig(businessId));

            if (string.IsNullOrEmpty(category)) return new NotFoundObjectResult("No category was supplied");

            try
            {
                // TODO: Change this to a service call
                var eventsByCategory = await repository.GetEventsByCategory(category, onlyNextOccurrence);
                var eventsByTag = await repository.GetEventsByTag(category, onlyNextOccurrence);

                if (eventsByCategory.Count == 0 && eventsByTag.Count == 0) return new NotFoundObjectResult($"No events found for category {category}");

                var events = eventsByCategory.Count > 0 ? eventsByCategory : eventsByTag;

                return new OkObjectResult(events);
            }
            catch (Exception ex)
            {
                _logger.LogError(new EventId(0), ex, $"There was an error with getting events by category / tag for category {category}");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpDelete]
        [Route("{businessId}/events/{slug}")]
        [Route("v1/{businessId}/events/{slug}")]
        public async Task<IActionResult> DeleteEvent(string slug, string businessId)
        {
            var repository = _eventRepository(_createConfig(businessId));
            var existingEvent = await repository.GetContentfulEvent(slug);

            return await _handler.Get(async () =>
            {
                var managementRepository = _managementRepository(_createConfig(businessId));
                var version = await managementRepository.GetVersion(existingEvent.Sys.Id);
                existingEvent.Sys.Version = version;
                return await managementRepository.Delete(existingEvent.Sys);
            });
        }

        private ManagementEvent ConvertToManagementEvent(Event eventDetail, List<ContentfulEventCategory> categories, ContentfulEvent existingEvent)
        {
            var contentfulEvent = _mapper.Map<ContentfulEvent>(eventDetail);
            contentfulEvent.EventCategories = categories;
            contentfulEvent.Image = existingEvent.Image;
            contentfulEvent.Group = existingEvent.Group;
            contentfulEvent.Documents = existingEvent.Documents;
            var managementEvent = new ManagementEvent();
            _mapper.Map(contentfulEvent, managementEvent);
            return managementEvent;
        }
    }
}