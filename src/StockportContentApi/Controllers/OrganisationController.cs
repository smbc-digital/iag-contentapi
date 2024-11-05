namespace StockportContentApi.Controllers;

public class OrganisationController : Controller
{
    // private readonly Func<string, ContentfulConfig> _createConfig;
    private readonly ResponseHandler _handler;
    private readonly Func<string, OrganisationRepository> _organisationRepository;

    public OrganisationController(ResponseHandler handler,
        // Func<string, ContentfulConfig> createConfig,
        Func<string, OrganisationRepository> organisationRepository)
    {
        _handler = handler;
        // _createConfig = createConfig;
        _organisationRepository = organisationRepository;
    }

    [HttpGet]
    [Route("v1/{businessId}/organisations/{organisationSlug}")]
    public async Task<IActionResult> GetOrganisation(string organisationSlug, string businessId)
    {
        return await _handler.Get(() =>
        {
            OrganisationRepository repository = _organisationRepository(businessId);
            Task<HttpResponse> article = repository.GetOrganisation(organisationSlug);

            return article;
        });
    }
}