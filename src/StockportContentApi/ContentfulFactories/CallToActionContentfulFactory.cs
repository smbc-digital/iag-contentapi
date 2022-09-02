using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class CallToActionContentfulFactory : IContentfulFactory<ContentfulCallToAction, CallToAction>
    {
        public CallToAction ToModel(ContentfulCallToAction entry)
        {
            if (entry is null)
                return null;

            return new CallToAction(entry.Title, entry.Text, entry.Link, entry.Image?.File?.Url);
        }
    }
}
