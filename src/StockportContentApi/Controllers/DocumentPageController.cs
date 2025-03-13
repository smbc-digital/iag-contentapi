namespace StockportContentApi.Controllers;

public class DocumentPageController(ResponseHandler handler,
                                    Func<string, IDocumentPageRepository> createRepository) : Controller
{
    private readonly Func<string, IDocumentPageRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/document-page/{documentPageSlug}")]
    [Route("v1/{businessId}/document-page/{documentPageSlug}")]
    public async Task<IActionResult> GetDocumentPage(string documentPageSlug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId).GetDocumentPage(documentPageSlug));
}