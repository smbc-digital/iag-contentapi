using System.Linq;
using Contentful.Core.Models;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Repositories;
using StockportContentApi.Utils;

namespace StockportContentApi.ContentfulFactories
{
    public class ApiKeyContentfulFactory : IContentfulFactory<ContentfulApiKey, ApiKey>
    {
        public ApiKey ToModel(ContentfulApiKey entryContentfulApiKey)
        {
            return new ApiKey(entryContentfulApiKey.Name, entryContentfulApiKey.Key, entryContentfulApiKey.Email,
                entryContentfulApiKey.ActiveFrom, entryContentfulApiKey.ActiveTo, entryContentfulApiKey.EndPoints, entryContentfulApiKey.Version);
        }
    }
}