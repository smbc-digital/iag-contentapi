namespace StockportContentApiTests.Unit.Builders;

internal class ContentfulServicePayPaymentBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _teaser = "teaser";
    private readonly string _description = "description";
    private readonly string _paymentDetailsText = "paymentDetailsText";
    private string _referenceLabel = "referenceLabel";
    private readonly string _metaDescription = "metaDescription";
    private readonly List<ContentfulAlert> _alerts = new();
    private readonly List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().Build() };
    private string _paymentAmount = "23.00";
    private readonly string _accountReference = "accountReference";
    private readonly string _catalogueId = "catalogueId";

    public ContentfulServicePayPayment Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            Description = _description,
            PaymentDetailsText = _paymentDetailsText,
            ReferenceLabel = _referenceLabel,
            Breadcrumbs = _breadcrumbs,
            MetaDescription = _metaDescription,
            Alerts = _alerts,
            PaymentAmount = _paymentAmount,
            AccountReference = _accountReference,
            CatalogueId = _catalogueId
        };

    public ContentfulServicePayPaymentBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulServicePayPaymentBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulServicePayPaymentBuilder Teaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }

    public ContentfulServicePayPaymentBuilder ReferenceLabel(string referenceLabel)
    {
        _referenceLabel = referenceLabel;
        return this;
    }

    public ContentfulServicePayPaymentBuilder PaymentAmount(string paymentAmount)
    {
        _paymentAmount = paymentAmount;
        return this;
    }

    public ContentfulServicePayPaymentBuilder AccountReference(string accountReference)
    {
        _paymentAmount = accountReference;
        return this;
    }

    public ContentfulServicePayPaymentBuilder CatalogueId(string catalogueId)
    {
        _paymentAmount = catalogueId;
        return this;
    }
}