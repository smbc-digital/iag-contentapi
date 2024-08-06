namespace StockportContentApi.Controllers;

public class PrivacyNoticeController : Controller
{
    private readonly ResponseHandler _handler;
    private readonly Func<ContentfulConfig, IPrivacyNoticeRepository> _privacyNoticeRepository;
    private readonly Func<string, ContentfulConfig> _createConfig;

    public PrivacyNoticeController(ResponseHandler handler, Func<ContentfulConfig, IPrivacyNoticeRepository> privacyNoticeRepository, Func<string, ContentfulConfig> createConfig)
    {
        _handler = handler;
        _createConfig = createConfig;
        _privacyNoticeRepository = privacyNoticeRepository;
    }

    [HttpGet]
    [Route("{businessId}/privacy-notices/{slug}")]
    [Route("v1/{businessId}/privacy-notices/{slug}")]
    public async Task<IActionResult> GetPrivacyNotice(string slug, string businessId) => 
        await _handler.Get(async () =>
        {
            IPrivacyNoticeRepository repository = _privacyNoticeRepository(_createConfig(businessId));
            PrivacyNotice privacyNotice = await repository.GetPrivacyNotice(slug);

            if (privacyNotice is null)
                return HttpResponse.Failure(HttpStatusCode.NotFound, "Privacy notice not found");

            return HttpResponse.Successful(privacyNotice);
        });

    [HttpGet]
    [Route("{businessId}/privacy-notices")]
    [Route("v1/{businessId}/privacy-notices")]
    public async Task<IActionResult> GetAllPrivacyNotices([FromRoute] string businessId) => 
        await _handler.Get(async () =>
        {
            IPrivacyNoticeRepository repository = _privacyNoticeRepository(_createConfig(businessId));

            List<PrivacyNotice> privacyNotices = await repository.GetAllPrivacyNotices();

            if (!privacyNotices.Any() || privacyNotices is null)
                return HttpResponse.Failure(HttpStatusCode.NotFound, "Privacy notices not found");
            else
                return HttpResponse.Successful(privacyNotices);
        });
}