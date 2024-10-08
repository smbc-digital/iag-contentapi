﻿namespace StockportContentApi.Controllers;

public class SectionController : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly Func<ContentfulConfig, SectionRepository> _createRepository;
    private readonly ResponseHandler _handler;

    public SectionController(ResponseHandler handler,
        Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, SectionRepository> createRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _createRepository = createRepository;
    }

    [HttpGet]
    [Route("{businessId}/sections/{sectionSlug}")]
    [Route("v1/{businessId}/sections/{sectionSlug}")]
    public async Task<IActionResult> GetSection(string sectionSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            SectionRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> section = repository.GetSections(sectionSlug);

            return section;
        });
    }

    [HttpGet]
    [Route("{businessId}/sectionsitemap")]
    [Route("v1/{businessId}/sectionsitemap")]
    public async Task<IActionResult> Get(string businessId)
    {
        return await _handler.Get(() =>
        {
            SectionRepository repository = _createRepository(_createConfig(businessId));
            Task<HttpResponse> section = repository.Get();

            return section;
        });
    }
}