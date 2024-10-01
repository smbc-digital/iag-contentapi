namespace StockportContentApi.Controllers;

public class EventController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, EventCategoryRepository> _eventCategoryRepository;
    private readonly Func<ContentfulConfig, EventRepository> _eventRepository;
    private readonly ResponseHandler _handler;
    private readonly ILogger<EventController> _logger;
    private readonly Func<ContentfulConfig, ManagementRepository> _managementRepository;
    private readonly IMapper _mapper;

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
            EventCategoryRepository eventRepository = _eventCategoryRepository(_createConfig(businessId));

            return eventRepository.GetEventCategories();
        });
    }

    [HttpGet]
    [Route("{businessId}/eventhomepage")]
    [Route("v1/{businessId}/eventhomepage")]
    public async Task<IActionResult> Homepage(string businessId)
    {
        EventCategoryRepository categoryRepository = _eventCategoryRepository(_createConfig(businessId));
        HttpResponse categoriesresponse = await categoryRepository.GetEventCategories();
        List<EventCategory> categories = categoriesresponse.Get<List<EventCategory>>();

        EventRepository repository = _eventRepository(_createConfig(businessId));
        HttpResponse response = await repository.GetEventHomepage();
        EventHomepage homepage = response.Get<EventHomepage>();
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
            EventRepository repository = _eventRepository(_createConfig(businessId));

            return repository.GetEvent(slug, date);
        });
    }

    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpPut]
    [Route("{businessId}/events/{slug}")]
    [Route("v1/{businessId}/events/{slug}")]
    public async Task<IActionResult> UpdateEvent([FromBody] Event eventDetail, string businessId)
    {
        EventRepository repository = _eventRepository(_createConfig(businessId));
        ContentfulEvent existingEvent = await repository.GetContentfulEvent(eventDetail.Slug);

        ContentfulCollection<ContentfulEventCategory> existingCategories =
            await repository.GetContentfulEventCategories();
        List<ContentfulEventCategory> referencedCategories = existingCategories.Items
            .Where(c => eventDetail.EventCategories.Any(ed => c.Name.Equals(ed.Name))).ToList();

        ManagementEvent managementEvent = ConvertToManagementEvent(eventDetail, referencedCategories, existingEvent);

        return await _handler.Get(async () =>
        {
            ManagementRepository managementRepository = _managementRepository(_createConfig(businessId));
            int version = await managementRepository.GetVersion(existingEvent.Sys.Id);
            existingEvent.Sys.Version = version;
            HttpResponse result = await managementRepository.CreateOrUpdate(managementEvent, existingEvent.Sys);

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
            EventRepository repository = _eventRepository(_createConfig(businessId));

            return repository.Get(dateFrom, dateTo, category, limit, featured, tag, price, latitude, longitude);
        });
    }

    [HttpGet]
    [Route("{businessId}/events/by-category")]
    [Route("v1/{businessId}/events/by-category")]
    public async Task<IActionResult> GetEventsByCatrgoryOrTag(string businessId, [FromQuery] string category = "",
        bool onlyNextOccurrence = true)
    {
        EventRepository repository = _eventRepository(_createConfig(businessId));

        if (string.IsNullOrEmpty(category)) return new NotFoundObjectResult("No category was supplied");

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
            _logger.LogError(new(0), ex,
                $"There was an error with getting events by category / tag for category {category}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }


    [ApiExplorerSettings(IgnoreApi = true)]
    [HttpDelete]
    [Route("{businessId}/events/{slug}")]
    [Route("v1/{businessId}/events/{slug}")]
    public async Task<IActionResult> DeleteEvent(string slug, string businessId)
    {
        EventRepository repository = _eventRepository(_createConfig(businessId));
        ContentfulEvent existingEvent = await repository.GetContentfulEvent(slug);

        return await _handler.Get(async () =>
        {
            ManagementRepository managementRepository = _managementRepository(_createConfig(businessId));
            int version = await managementRepository.GetVersion(existingEvent.Sys.Id);
            existingEvent.Sys.Version = version;

            return await managementRepository.Delete(existingEvent.Sys);
        });
    }

    private ManagementEvent ConvertToManagementEvent(Event eventDetail, List<ContentfulEventCategory> categories,
        ContentfulEvent existingEvent)
    {
        ContentfulEvent contentfulEvent = _mapper.Map<ContentfulEvent>(eventDetail);
        contentfulEvent.EventCategories = categories;
        contentfulEvent.Image = existingEvent.Image;
        contentfulEvent.Group = existingEvent.Group;
        contentfulEvent.Documents = existingEvent.Documents;
        ManagementEvent managementEvent = new();
        _mapper.Map(contentfulEvent, managementEvent);

        return managementEvent;
    }
}