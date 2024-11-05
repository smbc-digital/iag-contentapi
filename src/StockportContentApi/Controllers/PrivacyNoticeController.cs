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
    [Route("v1/{businessId}/privacy-notices/{slug}")]
    public async Task<IActionResult> GetPrivacyNotice(string slug, string businessId) =>
        await _handler.Get(async () =>
        {
            IPrivacyNoticeRepository repository = _privacyNoticeRepository(businessId);
            PrivacyNotice privacyNotice = await repository.GetPrivacyNotice(slug);

            if (privacyNotice is null)
                return HttpResponse.Failure(HttpStatusCode.NotFound, "Privacy notice not found");

            return HttpResponse.Successful(privacyNotice);
        });

    [HttpGet]
    [Route("v1/{businessId}/privacy-notices")]
    public async Task<IActionResult> GetAllPrivacyNotices([FromRoute] string businessId) =>
        await _handler.Get(async () =>
        {
            IPrivacyNoticeRepository repository = _privacyNoticeRepository(businessId);

            List<PrivacyNotice> privacyNotices = await repository.GetAllPrivacyNotices();

            if (!privacyNotices.Any() || privacyNotices is null)
                return HttpResponse.Failure(HttpStatusCode.NotFound, "Privacy notices not found");

            return HttpResponse.Successful(privacyNotices);
        });
}