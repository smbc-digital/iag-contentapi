namespace StockportContentApi.Model;

public class Payment
{
    public string Title { get; set; }
    public string Slug { get; set; }
    public string Teaser { get; set; }
    public string Description { get; set; }
    public string PaymentDetailsText { get; set; }
    public string ReferenceLabel { get; set; }
    public string ParisReference { get; set; }
    public string Fund { get; set; }
    public string GlCodeCostCentreNumber { get; set; }
    public IEnumerable<Crumb> Breadcrumbs { get; }
    public string Icon { get; set; }
    public EPaymentReferenceValidation ReferenceValidation { get; set; }
    public string MetaDescription { get; set; }
    public string ReturnUrl { get; set; }
    public string CatalogueId { get; set; }
    public string AccountReference { get; set; }
    public string PaymentDescription { get; set; }
    public IEnumerable<Alert> Alerts { get; }

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
        string paymentDescription,
        IEnumerable<Alert> alerts
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
        Alerts = alerts;
    }
}


