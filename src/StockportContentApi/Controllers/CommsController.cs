﻿namespace StockportContentApi.Controllers;

public class CommsController : Controller
{
    private readonly Func<ContentfulConfig, CommsRepository> _commsRepository;
    private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly ResponseHandler _handler;

    public CommsController(ResponseHandler handler, Func<string, ContentfulConfig> createConfig,
        Func<ContentfulConfig, CommsRepository> commsRepository)
    {
        _handler = handler;
        _createConfig = createConfig;
        _commsRepository = commsRepository;
    }

    [HttpGet]
    [Route("{businessId}/comms")]
    [Route("v1/{businessId}/comms")]
    public async Task<IActionResult> Get(string businessId) =>
        await _handler.Get(() => _commsRepository(_createConfig(businessId)).Get());
}