using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Payment
    {
        public string Title { get; }
        public string Slug { get; }
        public string Teaser { get; }
        public string Description { get; }
        public string PaymentDetailsText { get; }
        public string ReferenceLabel { get; }
        public string ParisReference { get; }
        public string Fund { get; }
        public string GlCodeCostCentreNumber { get; }
        public IEnumerable<Crumb> Breadcrumbs { get; }

        public Payment(string title,
            string slug,
            string teaser,
            string description,
            string paymentDetailsText,
            string referenceLabel,
            string parisReference,
            string fund,
            string glCodeCostCentreNumber,
            IEnumerable<Crumb> breadcrumbs
            )
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Description = description;
            PaymentDetailsText = paymentDetailsText;
            ReferenceLabel = referenceLabel;
            ParisReference = parisReference;
            Fund = fund;
            GlCodeCostCentreNumber = glCodeCostCentreNumber;
            Breadcrumbs = breadcrumbs;
        }
    }
}


