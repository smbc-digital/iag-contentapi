using StockportContentApi.Attributes;
using System.Collections.Generic;
using StockportContentApi.Enums;

namespace StockportContentApi.Model
{
    public class Payment
    {
        [SensitiveData]
        public string Title { get; set; }
        [SensitiveData]
        public string Slug { get; set; }
        [SensitiveData]
        public string Teaser { get; set; }
        [SensitiveData]
        public string Description { get; set; }
        [SensitiveData]
        public string PaymentDetailsText { get; set; }
        [SensitiveData]
        public string ReferenceLabel { get; set; }
        [SensitiveData]
        public string ParisReference { get; set; }
        [SensitiveData]
        public string Fund { get; set; }
        [SensitiveData]
        public string GlCodeCostCentreNumber { get; set; }
        public IEnumerable<Crumb> Breadcrumbs { get; }
        public string Icon { get; set; }
        public EPaymentReferenceValidation ReferenceValidation { get; set; }
        public string MetaDescription { get; set; }
        public string ReturnUrl { get; set; }
        public string CatalogueId { get; set; }
        public string AccountReference { get; set; }
        public string PaymentDescription { get; set; }

        public Payment(string title,
            string slug,
            string teaser,
            string description,
            string paymentDetailsText,
            string referenceLabel,
            string parisReference,
            string fund,
            string glCodeCostCentreNumber,
            string icon,
            IEnumerable<Crumb> breadcrumbs,
            EPaymentReferenceValidation referenceValidation,
            string metaDescription,
            string returnUrl,
            string catalogueId,
            string accountReference,
            string paymentDescription
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
            Icon = icon;
            Breadcrumbs = breadcrumbs;
            ReferenceValidation = referenceValidation;
            MetaDescription = metaDescription;
            ReturnUrl = returnUrl;
            CatalogueId = catalogueId;
            AccountReference = accountReference;
            PaymentDescription = paymentDescription;
        }
    }
}


