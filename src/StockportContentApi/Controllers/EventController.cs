namespace StockportContentApi.Controllers;

public class EventController(ResponseHandler handler,
                            Func<string, string, IEventRepository> eventRepository,
                            Func<string, string, IEventCategoryRepository> eventCategoryRepository,
                            Func<string, IManagementRepository> managementRepository,
                            IMapper mapper,
                            ILogger<EventController> logger) : Controller
{
    private readonly Func<string, string, IEventCategoryRepository> _eventCategoryRepository = eventCategoryRepository;
    private readonly Func<string, string, IEventRepository> _eventRepository = eventRepository;
    private readonly ResponseHandler _handler = handler;
    private readonly ILogger<EventController> _logger = logger;
    private readonly Func<string, IManagementRepository> _managementRepository = managementRepository;
    private readonly IMapper _mapper = mapper;

    [HttpGet]
    [Route("{businessId}/event-categories")]
    [Route("v1/{businessId}/event-categories")]
    public async Task<IActionResult> GetEventCategories(string businessId) =>
        await _handler.Get(() => _eventCategoryRepository(businessId, businessId).GetEventCategories());

    [HttpGet]
    [Route("{businessId}/eventhomepage")]
    [Route("v1/{businessId}/eventhomepage")]
    public async Task<IActionResult> Homepage(string businessId)
    {
        IEventCategoryRepository categoryRepository = _eventCategoryRepository(businessId, businessId);
        HttpResponse categoriesResponse = await categoryRepository.GetEventCategories();
        List<EventCategory> categories = categoriesResponse.Get<List<EventCategory>>();

        IEventRepository repository = _eventRepository(businessId, businessId);
        int quantityWithinHomepageRows = businessId.Equals("stockroom") ? 6 : 3;
        HttpResponse response = await repository.GetEventHomepage(quantityWithinHomepageRows);
        EventHomepage homepage = response.Get<EventHomepage>();
        homepage.Categories = categories;

        return Ok(homepage);
    }

    [HttpGet]
    [Route("{businessId}/events/{slug}")]
    [Route("v1/{businessId}/events/{slug}")]
    public async Task<IActionResult> Detail(string slug, string businessId, [FromQuery] DateTime? date) =>
        await _handler.Get(() => _eventRepository(businessId, businessId).GetEvent(slug, date));

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPut]
    [Route("{businessId}/events/{slug}")]
    [Route("v1/{businessId}/events/{slug}")]
    public async Task<IActionResult> UpdateEvent([FromBody] Event eventDetail, string businessId)
    {
        IEventRepository repository = _eventRepository(businessId, businessId);
        ContentfulEvent existingEvent = await repository.GetContentfulEvent(eventDetail.Slug);

        ContentfulCollection<ContentfulEventCategory> existingCategories = await repository.GetContentfulEventCategories();
        List<ContentfulEventCategory> referencedCategories = existingCategories.Items
            .Where(c => eventDetail.EventCategories.Any(ed => c.Name.Equals(ed.Name)))
            .ToList();

        ManagementEvent managementEvent = ConvertToManagementEvent(eventDetail, referencedCategories, existingEvent);

        return await _handler.Get(async () =>
        {
            IManagementRepository managementRepository = _managementRepository(businessId);
            existingEvent.Sys.Version = await managementRepository.GetVersion(existingEvent.Sys.Id);
            return await managementRepository.CreateOrUpdate(managementEvent, existingEvent.Sys);
        });
    }

    [HttpGet]
    [Route("{businessId}/events")]
    [Route("{businessId}/events/latest/{limit}")]
    [Route("v1/{businessId}/events")]
    [Route("v1/{businessId}/events/latest/{limit}")]
    public async Task<IActionResult> Index(string businessId,
                                                int limit = 0,
                                                [FromQuery] DateTime? dateFrom = null,
                                                [FromQuery] DateTime? dateTo = null,
                                                [FromQuery] string category = null,
                                                [FromQuery] bool? featured = null,
                                                [FromQuery] string tag = null,
                                                [FromQuery] string price = null,
                                                [FromQuery] double latitude = 0,
                                                [FromQuery] double longitude = 0,
                                                [FromQuery] bool? free = null) =>
        await _handler.Get(() => _eventRepository(businessId, businessId).Get(dateFrom, dateTo, category, limit, featured, tag, price, latitude, longitude, free));

    [HttpGet]
    [Route("{businessId}/events/by-category")]
    [Route("v1/{businessId}/events/by-category")]
    public async Task<IActionResult> GetEventsByCategoryOrTag(string businessId, [FromQuery] string category = "", bool onlyNextOccurrence = true)
    {
        IEventRepository repository = _eventRepository(businessId, businessId);

        if (string.IsNullOrEmpty(category))
            return new NotFoundObjectResult("No category was supplied");

        try
        {
            // TODO: Change this to a service call
            List<Event> eventsByCategory = await repository.GetEventsByCategory(category, onlyNextOccurrence);
            List<Event> eventsByTag = await repository.GetEventsByTag(category, onlyNextOccurrence);

            if (eventsByCategory.Count.Equals(0) && eventsByTag.Count.Equals(0))
                return new NotFoundObjectResult($"No events found for category {category}");

            List<Event> events = eventsByCategory.Count > 0
                ? eventsByCategory
                : eventsByTag;

            return new OkObjectResult(events);
        }
        catch (Exception ex)
        {
            _logger.LogError($"{nameof(EventController)}::{nameof(GetEventsByCategoryOrTag)}: " +
                $"An unexpected error occurred getting events by category / tag for category {category}" + 
                $"{ex.Message}");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete]
    [Route("{businessId}/events/{slug}")]
    [Route("v1/{businessId}/events/{slug}")]
    public async Task<IActionResult> DeleteEvent(string slug, string businessId)
    {
        IEventRepository repository = _eventRepository(businessId, businessId);
        ContentfulEvent existingEvent = await repository.GetContentfulEvent(slug);

        return await _handler.Get(async () =>
        {
            IManagementRepository managementRepository = _managementRepository(businessId);
            int version = await managementRepository.GetVersion(existingEvent.Sys.Id);
            existingEvent.Sys.Version = version;

            return await managementRepository.Delete(existingEvent.Sys);
        });
    }

    private ManagementEvent ConvertToManagementEvent(Event eventDetail, List<ContentfulEventCategory> categories, ContentfulEvent existingEvent)
    {
        ContentfulEvent contentfulEvent = _mapper.Map<ContentfulEvent>(eventDetail);
        contentfulEvent.EventCategories = categories;
        contentfulEvent.Image = existingEvent.Image;
        contentfulEvent.Documents = existingEvent.Documents;
        ManagementEvent managementEvent = new();
        _mapper.Map(contentfulEvent, managementEvent);

        return managementEvent;
    }
}