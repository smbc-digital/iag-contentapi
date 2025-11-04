namespace StockportContentApi.Controllers;

public class TopicController(ResponseHandler handler,
                            Func<string, ITopicRepository> createRepository) : Controller
{
    private readonly Func<string, ITopicRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/topics/{topicSlug}")]
    [Route("v1/{businessId}/topics/{topicSlug}")]
    public async Task<IActionResult> GetTopicByTopicSlug(string businessId, string topicSlug) =>
        await _handler.Get(() => _createRepository(businessId).GetTopicByTopicSlug(topicSlug, businessId));

    [HttpGet]
    [Route("{businessId}/topics/")]
    [Route("v1/{businessId}/topics/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).Get(businessId));
}