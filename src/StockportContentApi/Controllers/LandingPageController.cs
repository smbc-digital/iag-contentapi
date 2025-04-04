﻿namespace StockportContentApi.Controllers;

public class LandingPageController(ResponseHandler handler,
                                Func<string, string, ILandingPageRepository> createRepository) : Controller
{
    private readonly Func<string, string, ILandingPageRepository> _createRepository = createRepository;
    private readonly ResponseHandler _handler = handler;

    [HttpGet]
    [Route("{businessId}/landing/{slug}")]
    [Route("v1/{businessId}/landing/{slug}")]
    public async Task<IActionResult> GetLandingPage(string slug, string businessId) =>
        await _handler.Get(() => _createRepository(businessId, businessId).GetLandingPage(slug));
}