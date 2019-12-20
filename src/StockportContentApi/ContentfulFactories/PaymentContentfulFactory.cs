using System.Linq;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Microsoft.AspNetCore.Http;

namespace StockportContentApi.ContentfulFactories
{
    public class PaymentContentfulFactory : IContentfulFactory<ContentfulPayment, Payment>
    {

        private readonly IContentfulFactory<ContentfulReference, Crumb> _crumbFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public PaymentContentfulFactory(IContentfulFactory<ContentfulReference, Crumb> crumbFactory, IHttpContextAccessor httpContextAccessor)
        {
            _crumbFactory = crumbFactory;
            _httpContextAccessor = httpContextAccessor;
        }

        public Payment ToModel(ContentfulPayment entry)
        {
            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.Sys))
                                              .Select(crumb => _crumbFactory.ToModel(crumb)).ToList();

            return new Payment(entry.Title,
                entry.Slug,
                entry.Teaser,
                entry.Description,
                entry.PaymentDetailsText,
                entry.ReferenceLabel,
                entry.ParisReference,
                entry.Fund,
                entry.GlCodeCostCentreNumber,
                entry.Icon,
                breadcrumbs,
                entry.ReferenceValidation,
                entry.MetaDescription,
                entry.ReturnUrl,
                entry.CatalogueId,
                entry.PaymentDescription
                ).StripData(_httpContextAccessor);
        }
    }
}