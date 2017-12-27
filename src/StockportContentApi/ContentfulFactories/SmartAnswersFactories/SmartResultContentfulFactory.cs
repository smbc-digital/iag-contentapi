using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories.SmartAnswersFactories
{
    public class SmartResultContentfulFactory : IContentfulFactory<ContentfulSmartResult, SmartResult>
    {
        public SmartResult ToModel(ContentfulSmartResult entry)
        {
            var buttonText = string.IsNullOrEmpty(entry.ButtonText) ? "Go to homepage" : entry.ButtonText;
            var buttonLink = string.IsNullOrEmpty(entry.ButtonLink) ? "https://www.stockport.gov.uk/" : entry.ButtonLink;

            return new SmartResult(entry.Title, entry.Slug, entry.Subheading, entry.Icon, entry.Body, buttonText, buttonLink);         
        }
    }
}
