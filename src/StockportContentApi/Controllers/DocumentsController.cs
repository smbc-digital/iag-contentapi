﻿namespace StockportContentApi.Controllers;

public class DocumentsController(IDocumentService documentService,
                                ILogger<DocumentsController> logger) : Controller
{
    private readonly IDocumentService _documentService = documentService;
    private readonly ILogger<DocumentsController> _logger = logger;

    [HttpGet]
    [Route("{businessId}/documents/{assetId}")]
    public async Task<IActionResult> GetSecureDocument(string businessId, string assetId)
    {
        try
        {
            Document result = await _documentService.GetSecureDocumentByAssetId(businessId, assetId);

            if (result is null)
                return new NotFoundObjectResult($"No document found for assetId {assetId}");

            return new OkObjectResult(result);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error getting secrure document with assetId: {assetId} with exception: {ex.Message}");

            return StatusCode(StatusCodes.Status500InternalServerError);
        }
    }
}