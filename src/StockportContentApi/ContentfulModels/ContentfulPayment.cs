﻿using System.Collections.Generic;
using StockportContentApi.Enums;
using StockportContentApi.Model;

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
        public EPaymentReferenceValidation ReferenceValidation { get; set; } = EPaymentReferenceValidation.None;
        public List<ContentfulReference> Breadcrumbs { get; set; } = new List<ContentfulReference>();
        public string MetaDescription { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CatalogueId { get; set; } = string.Empty;
        public string AccountReference { get; set; } = string.Empty;
        public string PaymentDescription { get; set; } = string.Empty;
        public List<ContentfulAlert> Alerts { get; set; } = new List<ContentfulAlert>();
    }
}
