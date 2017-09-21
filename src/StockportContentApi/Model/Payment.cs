using StockportContentApi.Attributes;
using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Payment
    {
        [SensitiveData]
        public string Title { get; }
        [SensitiveData]
        public string Slug { get; }
        [SensitiveData]
        public string Teaser { get; }
        [SensitiveData]
        public string Description { get; }
        [SensitiveData]
        public string PaymentDetailsText { get; }
        [SensitiveData]
        public string ReferenceLabel { get; }
        [SensitiveData]
        public string ParisReference { get; }
        [SensitiveData]
        public string Fund { get; }
        [SensitiveData]
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


