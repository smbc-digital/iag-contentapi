using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.ContentfulFactories
{
    public class KeyFactContentfulFactory : IContentfulFactory<ContentfulKeyFact, KeyFact>
    {
        public KeyFact ToModel(ContentfulKeyFact entry)
        {
            return new KeyFact(entry.Icon, entry.Text, entry.Link);
        }
    }
}