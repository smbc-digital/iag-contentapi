using StockportContentApi.ContentfulModels;

namespace StockportContentApiTests.Unit.Repositories
{
    internal class ContentfulPaymentBuilder
    {
        private string _title = "title";
        private string _slug = "slug";
        private string _description = "description";
        private string _paymentDetailsText = "paymentDetailsText";
        private string _referenceLabel = "referenceLabel";
        private string _parisReference = "parisReference";
        private string _fund = "fund";
        private string _glCodeCostCentreNumber = "glCodeCostCentreNumber";

        public ContentfulPayment Build()
        {
            return new ContentfulPayment
            {
                Title = _title,
                Slug = _slug,
                Description = _description,
                PaymentDetailsText = _paymentDetailsText,
                ReferenceLabel = _referenceLabel,
                ParisReference = _parisReference,
                Fund = _fund,
                GlCodeCostCentreNumber = _glCodeCostCentreNumber
            };
        }

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

        public ContentfulPaymentBuilder Description(string description)
        {
            _description = description;
            return this;
        }

        public ContentfulPaymentBuilder PaymentDetailsText(string paymentDetailsText)
        {
            _paymentDetailsText = paymentDetailsText;
            return this;
        }

        public ContentfulPaymentBuilder ReferenceLabel(string referenceLabel)
        {
            _referenceLabel = referenceLabel;
            return this;
        }

        public ContentfulPaymentBuilder ParisReference(string parisReference)
        {
            _parisReference = parisReference;
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
    }
}