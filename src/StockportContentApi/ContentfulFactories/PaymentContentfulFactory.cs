using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;


namespace StockportContentApi.ContentfulFactories
{
    public class PaymentContentfulFactory : IContentfulFactory<ContentfulPayment, Payment>
    {

        private readonly IContentfulFactory<Entry<ContentfulCrumb>, Crumb> _crumbFactory;

        public PaymentContentfulFactory(IContentfulFactory<Entry<ContentfulCrumb>, Crumb> crumbFactory)
        {
            _crumbFactory = crumbFactory;
        }

        public Payment ToModel(ContentfulPayment entry)
        {

            var breadcrumbs = entry.Breadcrumbs.Where(section => ContentfulHelpers.EntryIsNotALink(section.SystemProperties))
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
                breadcrumbs);
        }
    }
}