namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class ServicePayPayment(string title,
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
                            IEnumerable<Alert> alerts)
{
    public string Title { get; set; } = title;
    public string Slug { get; set; } = slug;
    public string Teaser { get; set; } = teaser;
    public string Description { get; set; } = description;
    public string PaymentDetailsText { get; set; } = paymentDetailsText;
    public string ReferenceLabel { get; set; } = referenceLabel;
    public IEnumerable<Crumb> Breadcrumbs { get; } = breadcrumbs;
    public string Icon { get; set; } = icon;
    public EPaymentReferenceValidation ReferenceValidation { get; set; } = referenceValidation;
    public string MetaDescription { get; set; } = metaDescription;
    public string ReturnUrl { get; set; } = returnUrl;
    public string CatalogueId { get; set; } = catalogueId;
    public string AccountReference { get; set; } = accountReference;
    public string PaymentDescription { get; set; } = paymentDescription;
    public IEnumerable<Alert> Alerts { get; } = alerts;
}