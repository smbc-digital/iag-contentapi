using StockportContentApi.Model;

namespace StockportContentApi.Factories
{
    public class PaymentFactory : IFactory<Payment>
    {
        private readonly IBuildContentTypesFromReferences<Crumb> _breadcrumbFactory;

        public PaymentFactory(IBuildContentTypesFromReferences<Crumb> breadcrumbFactory)
        {
            _breadcrumbFactory = breadcrumbFactory;
        }

        public Payment Build(dynamic entry, IContentfulIncludes contentfulResponse)
        {
            var fields = entry.fields;
            if (fields == null)
                return null;


            string title = fields.title;
            string slug = fields.slug;
            string description = fields.description;
            string paymentDetailsText = fields.paymentDetailsText;
            string referenceLabel = fields.referenceLabel;
            string parisReference = fields.parisReference;
            string fund = fields.fund;
            string glCodeCostCentreNumber = fields.glCodeCostCentreNumber;


            return new Payment(title, slug, description, paymentDetailsText, referenceLabel, parisReference, fund, glCodeCostCentreNumber);
        }
    }
}