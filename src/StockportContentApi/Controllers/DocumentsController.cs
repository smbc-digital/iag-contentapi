using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
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
    }
}
