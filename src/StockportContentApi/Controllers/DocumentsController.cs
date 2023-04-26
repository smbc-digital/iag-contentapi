namespace StockportContentApi.Controllers;

public class DocumentsController : Controller
{
    private readonly IDocumentService _documentService;
    private readonly ILogger<DocumentsController> _logger;

    public DocumentsController(IDocumentService documentService, ILogger<DocumentsController> logger)
    {
        _documentService = documentService;
        _logger = logger;
    }

    [HttpGet]
    [Route("{businessId}/documents/{groupSlug}/{assetId}")]
    public async Task<IActionResult> GetSecureDocument(string businessId, string groupSlug, string assetId)
    {
        try
        {
            var result = await _documentService.GetSecureDocumentByAssetId(businessId, assetId, groupSlug);

            if (result == null) return new NotFoundObjectResult($"No document found for assetId {assetId}");

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting secrure document with assetId: {assetId} with exception: {ex.Message}");
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}
