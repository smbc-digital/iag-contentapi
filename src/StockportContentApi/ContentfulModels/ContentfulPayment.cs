using System.Collections.Generic;

namespace StockportContentApi.ContentfulModels
{
    public class ContentfulPayment : ContentfulReference
    {
        public string Description { get; set; } = string.Empty;
        public string PaymentDetailsText { get; set; } = string.Empty;
        public string ReferenceLabel { get; set; } = string.Empty;
        public string ParisReference { get; set; } = string.Empty;
        public string Fund { get; set; } = string.Empty;
        public string GlCodeCostCentreNumber { get; set; } = string.Empty;
        public new string Icon { get; set; } = "si-coin";
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
    }
}
