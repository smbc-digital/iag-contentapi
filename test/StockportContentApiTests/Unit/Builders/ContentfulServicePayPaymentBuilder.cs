﻿namespace StockportContentApiTests.Unit.Builders;

internal class ContentfulServicePayPaymentBuilder
{
    private string _title = "title";
    private string _slug = "slug";
    private string _teaser = "teaser";
    private readonly string _description = "description";
    private readonly string _paymentDetailsText = "paymentDetailsText";
    private string _referenceLabel = "referenceLabel";
    private readonly string _metaDescription = "metaDescription";
    private List<ContentfulAlert> _alerts = new();
    private List<ContentfulReference> _breadcrumbs = new() { new ContentfulReferenceBuilder().Build() };
    private string _accountReference = "accountReference";
    private string _catalogueId = "catalogueId";

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

    public ContentfulServicePayPaymentBuilder Breadcrumbs(List<ContentfulReference> breadcrumbs)
    {
        _breadcrumbs = breadcrumbs;
        return this;
    }

    public ContentfulServicePayPaymentBuilder Alerts(List<ContentfulAlert> alerts)
    {
        _alerts = alerts;
        return this;
    }

    public ContentfulServicePayPaymentBuilder AccountReference(string accountReference)
    {
        _accountReference = accountReference;
        return this;
    }

    public ContentfulServicePayPaymentBuilder CatalogueId(string catalogueId)
    {
        _catalogueId = catalogueId;
        return this;
    }
}