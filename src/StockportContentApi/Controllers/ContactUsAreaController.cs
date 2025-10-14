namespace StockportContentApi.Controllers;

public class ContactUsController(ResponseHandler handler,
                                Func<string, IContactUsAreaRepository> createRepository) : Controller
{
    private readonly Func<string, IContactUsAreaRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/contactusarea")]
    [Route("v1/{businessId}/contactusarea")]
    public async Task<IActionResult> GetContactUsArea(string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetContactUsArea(businessId));
}