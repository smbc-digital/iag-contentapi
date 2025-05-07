namespace StockportContentApiTests.Unit.Builders;

internal class ContentfulPaymentBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _teaser = "teaser";
    private readonly string _description = "description";
    private readonly string _paymentDetailsText = "paymentDetailsText";
    private string _referenceLabel = "referenceLabel";
    private string _fund = "fund";
    private string _glCodeCostCentreNumber = "glCodeCostCentreNumber";
    private readonly string _metaDescription = "metaDescription";
    private List<ContentfulAlert> _alerts = new();
    private List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().Build() };

    public ContentfulPayment Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            Description = _description,
            PaymentDetailsText = _paymentDetailsText,
            ReferenceLabel = _referenceLabel,
            Fund = _fund,
            GlCodeCostCentreNumber = _glCodeCostCentreNumber,
            Breadcrumbs = _breadcrumbs,
            MetaDescription = _metaDescription,
            Alerts = _alerts
        };

    public ContentfulPaymentBuilder Slug(string slug)
    {
        _slug = slug;
        return this;
    }

    public ContentfulPaymentBuilder Title(string title)
    {
        _title = title;
        return this;
    }

    public ContentfulPaymentBuilder Teaser(string teaser)
    {
        _teaser = teaser;
        return this;
    }

    public ContentfulPaymentBuilder ReferenceLabel(string referenceLabel)
    {
        _referenceLabel = referenceLabel;
        return this;
    }

    public ContentfulPaymentBuilder Fund(string fund)
    {
        _fund = fund;
        return this;
    }
    public ContentfulPaymentBuilder GlCodeCostCentreNumber(string glCodeCostCentreNumber)
    {
        _glCodeCostCentreNumber = glCodeCostCentreNumber;
        return this;
    }

    public ContentfulPaymentBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
    {
        _breadcrumbs = breadcrumbs;
        return this;
    }

    public ContentfulPaymentBuilder Alerts(List<ContentfulAlert> alerts)
    {
        _alerts = alerts;
        return this;
    }
}