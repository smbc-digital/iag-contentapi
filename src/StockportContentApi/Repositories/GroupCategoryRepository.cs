using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public class GroupCategoryRepository
    {
        private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public GroupCategoryRepository(ContentfulConfig config, IContentfulFactory<ContentfulGroupCategory, GroupCategory> contentfulFactory, IContentfulClientManager contentfulClientManager)
        {
            _contentfulFactory = contentfulFactory;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetGroupCategories()
        {
            var builder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory");

            var entries = await _client.GetEntriesAsync(builder);
            if (entries == null || !entries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No group catogories found");

            var groupCategories = entries.Select(groupCatogory => _contentfulFactory.ToModel(groupCatogory)).ToList();

            return HttpResponse.Successful(groupCategories);
        }
    }
}
