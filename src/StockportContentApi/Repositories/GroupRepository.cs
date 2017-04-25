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

        public async Task<HttpResponse> GetGroupResults(string category, double latitude, double longitude, string order)
        {
            var groupResults = new GroupResults();

            var builder =
                new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
                    .Include(1)
                    .Limit(ContentfulQueryValues.LIMIT_MAX);

            if (longitude != 0 && latitude != 0)
            {
                builder = builder.FieldEquals("fields.mapPosition[near]", latitude + "," + longitude + (latitude == 53.40581278523235 && longitude == -2.158041000366211 ? ",10" : ",3.2"));
            }

            //if (lat != 53.40581278523235 && lon != -2.158041000366211)
            //{
            //    builder = builder.FieldEquals("fields.mapPosition[near]", lat + "," + lon + ",3.2");
            //}
            //else
            //{
            //    builder = builder.FieldEquals("fields.mapPosition[near]", lat + "," + lon + ",10");
            //}

            var entries = await _client.GetEntriesAsync(builder);
            if (entries == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No groups found");

            var groups = _groupListFactory.ToModel(entries.ToList())
                .Where(g => g.CategoriesReference.Any(c => string.IsNullOrEmpty(category) || c.Slug.ToLower() == category.ToLower()))
                .ToList();

            switch (!string.IsNullOrEmpty(order) ? order.ToLower() : "name a-z")
            {
                case "name a-z":
                    groups = groups.OrderBy(g => g.Name).ToList();
                    break;
                case "name z-a":
                    groups = groups.OrderByDescending(g => g.Name).ToList();
                    break;
                case "nearest":
                    break;
                default:
                    groups = groups.OrderBy(g => g.Name).ToList();
                    break;
            }

            groupResults.Groups = groups;

            var groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);

            var groupCategoryEntries = await _client.GetEntriesAsync(groupCategoryBuilder);

            if (groupCategoryEntries != null || groupCategoryEntries.Any())
            {
                if (!string.IsNullOrEmpty(category) && !groupCategoryEntries.Any(g => g.Slug.ToLower() == category.ToLower()))
                    return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories found");

                var groupCategoryResults = _groupCategoryListFactory.ToModel(groupCategoryEntries.ToList())
                .ToList();

                groupResults.Categories = groupCategoryResults;
            }

            return HttpResponse.Successful(groupResults);
        }
    }
}
