using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class PaymentContentfulFactory : IContentfulFactory<ContentfulPayment, Payment>
    {
        public Payment ToModel(ContentfulPayment entry)
        {
            return new Payment(entry.Title, 
                               entry.Slug, 
                               entry.Description,
                               entry.PaymentDetailsText,
                               entry.ReferenceLabel,
                               entry.ParisReference,
                               entry.Fund,
                               entry.GlCodeCostCentreNumber);  
        }
    }
}