using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class GroupRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;
        private readonly IContentfulFactory<List<ContentfulGroup>, List<Group>> _groupListFactory;
        private readonly IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>> _groupCategoryListFactory;

        public GroupRepository(ContentfulConfig config, IContentfulClientManager clientManager, 
                                 IContentfulFactory<ContentfulGroup, Group> groupFactory,
                                 IContentfulFactory<List<ContentfulGroup>, List<Group>> groupListFactory,
                                 IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>> groupCategoryListFactory)
        {
            _client = clientManager.GetClient(config);
            _groupFactory = groupFactory;
            _groupListFactory = groupListFactory;
            _groupCategoryListFactory = groupCategoryListFactory;
        }

        public async Task<HttpResponse> GetGroup(string slug)
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            return entry == null 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No group found for '{slug}'") 
                : HttpResponse.Successful(_groupFactory.ToModel(entry));
        }

        public async Task<HttpResponse> GetGroupResults(string categorySlug)
        {
            var groupResults = new GroupResults();

            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);

            var entries = await _client.GetEntriesAsync(builder);
            if (entries == null || !entries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No groups found");

            var groups = _groupListFactory.ToModel(entries.ToList())
                .Where(g => g.CategoriesReference.Any(c => string.IsNullOrEmpty(categorySlug) || c.Slug == categorySlug))
                .OrderBy(g => g.Name)
                .ToList();

            groupResults.Groups = groups;

            var groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);

            var groupCategoryEntries = await _client.GetEntriesAsync(groupCategoryBuilder);

            if (groupCategoryEntries != null || groupCategoryEntries.Any())
            {
                if(!string.IsNullOrEmpty(categorySlug) && !groupCategoryEntries.Any(g => g.Slug == categorySlug))
                    return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories found");

                var groupCategoryResults = _groupCategoryListFactory.ToModel(groupCategoryEntries.ToList())
                .ToList();

                groupResults.Categories = groupCategoryResults;
            }

            return HttpResponse.Successful(groupResults);
        }
    }
}
