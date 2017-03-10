using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class Payment
    {
        public string Title { get; }
        public string Slug { get; }
        public string Description { get; }
        public string PaymentDetailsText { get; }
        public string ReferenceLabel { get; }
        public string ParisReference { get; }
        public string Fund { get; }
        public string GlCodeCostCentreNumber { get; }

        public Payment(string title,
            string slug,
            string description,
            string paymentDetailsText,
            string referenceLabel,
            string parisReference,
            string fund,
            string glCodeCostCentreNumber)
        {
            Title = title;
            Slug = slug;
            Description = description;
            PaymentDetailsText = paymentDetailsText;
            ReferenceLabel = referenceLabel;
            ParisReference = parisReference;
            Fund = fund;
            GlCodeCostCentreNumber = glCodeCostCentreNumber;
        }
    }
}


