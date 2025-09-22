namespace StockportContentApiTests.Unit.Builders;

internal class ContentfulPaymentBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _teaser = "teaser";
    private string _referenceLabel = "referenceLabel";
    private readonly List<ContentfulAlert> _alerts = new();
    private readonly List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().Build() };

    public ContentfulPayment Build()
        => new()
        {
            Title = _title,
            Slug = _slug,
            Teaser = _teaser,
            Description = "description",
            PaymentDetailsText = "paymentDetailsText",
            ReferenceLabel = _referenceLabel,
            Fund = "fund",
            GlCodeCostCentreNumber = "glCodeCostCentreNumber",
            Breadcrumbs = _breadcrumbs,
            MetaDescription = "metaDescription",
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
}