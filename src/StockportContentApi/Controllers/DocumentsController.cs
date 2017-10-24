using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Services;

namespace StockportContentApi.Controllers
{
    public class DocumentsController : Controller
    {
        private readonly IDocumentService _documentService;

        public DocumentsController(IDocumentService documentService)
        {
            _documentService = documentService;
        }

        [HttpGet]
        [Route("{businessId}/documents/{assetId}")]
        public async Task<IActionResult> Get(string businessId, string assetId)
        {
            var result = await _documentService.GetDocumentByAssetId(businessId, assetId);

            if (result == null) return new NotFoundObjectResult($"No document found for assetId {assetId}");

            return new OkObjectResult(result);
        }

        // TODO: Possibly move this to the webapp instead
        [HttpGet]
        [Route("{businessId}/documents/{groupSlug}/{assetId}")]
        public async Task<IActionResult> GetSecureDocument(string businessId, string groupSlug, string assetId)
        {
            var result = await _documentService.GetSecureDocumentByAssetId(businessId, assetId, groupSlug);

            if (result == null) return new NotFoundObjectResult($"No document found for assetId {assetId}");

            return new OkObjectResult(result);
        }
    }
}
