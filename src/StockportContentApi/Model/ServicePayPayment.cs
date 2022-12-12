using StockportContentApi.Enums;

namespace StockportContentApi.Model
{
    public class ServicePayPayment
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Teaser { get; set; }
        public string Description { get; set; }
        public string PaymentDetailsText { get; set; }
        public string ReferenceLabel { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public string Icon { get; set; }
        public EPaymentReferenceValidation ReferenceValidation { get; set; }
        public string MetaDescription { get; set; }
        public string ReturnUrl { get; set; }
        public string CatalogueId { get; set; }
        public string AccountReference { get; set; }
        public string PaymentDescription { get; set; }
        public IEnumerable<Alert> Alerts { get; }
        public string PaymentAmount { get; set; }

        public ServicePayPayment(
            string title,
            string slug,
            string teaser,
            string description,
            string paymentDetailsText,
            string referenceLabel,
            string icon,
            IEnumerable<Crumb> breadcrumbs,
            EPaymentReferenceValidation referenceValidation,
            string metaDescription,
            string returnUrl,
            string catalogueId,
            string accountReference,
            string paymentDescription,
            IEnumerable<Alert> alerts,
            string paymentAmount
            )
        {
            Title = title;
            Slug = slug;
            Teaser = teaser;
            Description = description;
            PaymentDetailsText = paymentDetailsText;
            ReferenceLabel = referenceLabel;
            Icon = icon;
            Breadcrumbs = breadcrumbs;
            ReferenceValidation = referenceValidation;
            MetaDescription = metaDescription;
            ReturnUrl = returnUrl;
            CatalogueId = catalogueId;
            AccountReference = accountReference;
            PaymentDescription = paymentDescription;
            Alerts = alerts;
            PaymentAmount = paymentAmount;
        }
    }
}
