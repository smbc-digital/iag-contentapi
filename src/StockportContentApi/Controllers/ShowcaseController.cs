﻿namespace StockportContentApi.Controllers;

[ExcludeFromCodeCoverage]
public class ShowcaseController(ResponseHandler handler,
                                Func<string, string, IShowcaseRepository> createRepository) : Controller
{
    private readonly Func<string, string, IShowcaseRepository> _createRepository = createRepository;

    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/showcases/{showcaseSlug}")]
    [Route("v1/{businessId}/showcases/{showcaseSlug}")]
    public async Task<IActionResult> GetShowcase(string showcaseSlug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId, businessId).GetShowcases(showcaseSlug));

    [HttpGet]
    [Route("{businessId}/showcases/")]
    [Route("v1/{businessId}/showcases/")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _createRepository(businessId, businessId).Get());
}