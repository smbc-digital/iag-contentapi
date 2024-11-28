namespace StockportContentApi.Controllers;

public class PrivacyNoticeController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<string, IPrivacyNoticeRepository> _privacyNoticeRepository;

    public PrivacyNoticeController(ResponseHandler handler,
        Func<string, IPrivacyNoticeRepository> privacyNoticeRepository)
    {
        _handler = handler;
        _privacyNoticeRepository = privacyNoticeRepository;
    }

    [HttpGet]
    [Route("{businessId}/privacy-notices/{slug}")]
    [Route("v1/{businessId}/privacy-notices/{slug}")]
    public async Task<IActionResult> GetPrivacyNotice(string slug, string businessId) =>
        await _handler.Get(async () =>
        {
            IPrivacyNoticeRepository repository = _privacyNoticeRepository(businessId);
            
            return await repository.GetPrivacyNotice(slug);

        });

    [HttpGet]
    [Route("{businessId}/privacy-notices")]
    [Route("v1/{businessId}/privacy-notices")]
    public async Task<IActionResult> GetAllPrivacyNotices([FromRoute] string businessId) =>
        await _handler.Get(async () =>
        {
            IPrivacyNoticeRepository repository = _privacyNoticeRepository(businessId);

            return await repository.GetAllPrivacyNotices();
        });
}