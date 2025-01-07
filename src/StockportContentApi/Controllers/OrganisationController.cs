namespace StockportContentApi.Controllers;

public class OrganisationController(ResponseHandler handler,
                                    Func<string, ContentfulConfig> createConfig,
                                    Func<string, CacheKeyConfig> createCacheKeyConfig,
                                    Func<ContentfulConfig, OrganisationRepository> organisationRepository) : Controller
{
    private readonly Func<string, ContentfulConfig> _createConfig = createConfig;
    private readonly Func<string, CacheKeyConfig> _createCacheKeyConfig = createCacheKeyConfig;
    private readonly ResponseHandler _handler = handler;
    private readonly Func<ContentfulConfig, OrganisationRepository> _organisationRepository = organisationRepository;

    [HttpGet]
    [Route("{businessId}/organisations/{organisationSlug}")]
    [Route("v1/{businessId}/organisations/{organisationSlug}")]
    public async Task<IActionResult> GetOrganisation(string organisationSlug, string businessId) =>
        await _handler.Get(() =>
        {
            OrganisationRepository repository = _organisationRepository(_createConfig(businessId));

            ContentfulConfig contentfulConfig = _createConfig(businessId);
            CacheKeyConfig cacheKeyConfig = _createCacheKeyConfig(businessId);

            Task<HttpResponse> organisation = repository.GetOrganisation(organisationSlug, contentfulConfig, cacheKeyConfig);

            return organisation;
        });
}