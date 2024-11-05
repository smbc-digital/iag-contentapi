namespace StockportContentApi.Controllers;

public class TopicController : Controller
{
    private readonly Func<string, TopicRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public TopicController(
        ResponseHandler handler,
        Func<string, TopicRepository> createRepository)
    {
        _handler = handler;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/topics/{topicSlug}")]
    [Route("v1/{businessId}/topics/{topicSlug}")]
    public async Task<IActionResult> GetTopicByTopicSlug(string businessId, string topicSlug) =>
        await _handler.Get(() =>
        {
            TopicRepository topicRepository = _createRepository(businessId);
            return topicRepository.GetTopicByTopicSlug(topicSlug);
        });

    [HttpGet]
    [Route("{businessId}/topics/")]
    [Route("v1/{businessId}/topics/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() =>
        {
            TopicRepository topicRepository = _createRepository(businessId);
            return topicRepository.Get();
        });
}