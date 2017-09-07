using StockportContentApi.Repositories;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using StockportContentApi.Config;

namespace StockportContentApi.Controllers
{
    public class OrganisationController : Controller
    {
        private readonly ResponseHandler _handler;
        private readonly Func<ContentfulConfig, OrganisationRepository> _organisationRepository;
        private readonly Func<string, ContentfulConfig> _createConfig;

        public OrganisationController(ResponseHandler handler,
            Func<string, ContentfulConfig> createConfig,
            Func<ContentfulConfig, OrganisationRepository> organisationRepository)
        {
            _handler = handler;
            _createConfig = createConfig;
            _organisationRepository = organisationRepository;
        }

        [HttpGet]
        [Route("api/{businessId}/organisations/{organisationSlug}")]
        public async Task<IActionResult> GetOrganisation(string organisationSlug, string  businessId)
        {
            return await _handler.Get(() =>
            {
                var repository = _organisationRepository(_createConfig(businessId));
                var article = repository.GetOrganisation(organisationSlug);

                return article;
            });
        }
    }
}