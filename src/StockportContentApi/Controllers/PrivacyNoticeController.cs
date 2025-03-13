namespace StockportContentApi.Controllers;

public class PrivacyNoticeController(ResponseHandler handler,
                                    Func<string, IPrivacyNoticeRepository> privacyNoticeRepository) : Controller
{
    private readonly ResponseHandler _handler = handler;
    private readonly Func<string, IPrivacyNoticeRepository> _privacyNoticeRepository = privacyNoticeRepository;

    [HttpGet]
    [Route("{businessId}/privacy-notices/{slug}")]
    [Route("v1/{businessId}/privacy-notices/{slug}")]
    public async Task<IActionResult> GetPrivacyNotice(string slug, string businessId) =>
        await _handler.Get(async () => await _privacyNoticeRepository(businessId).GetPrivacyNotice(slug));

    [HttpGet]
    [Route("{businessId}/privacy-notices")]
    [Route("v1/{businessId}/privacy-notices")]
    public async Task<IActionResult> GetAllPrivacyNotices([FromRoute] string businessId) =>
        await _handler.Get(async () => await _privacyNoticeRepository(businessId).GetAllPrivacyNotices());
}