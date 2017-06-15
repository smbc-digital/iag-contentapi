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
using Microsoft.Extensions.Caching.Memory;
using StockportContentApi.Client;
using StockportContentApi.Factories;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class GroupRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;
        private readonly IContentfulFactory<List<ContentfulGroup>, List<Group>> _groupListFactory;
        private readonly IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>> _groupCategoryListFactory;
        private readonly EventRepository _eventRepository;
        private readonly ICache _cache;


        public GroupRepository(ContentfulConfig config, IContentfulClientManager clientManager,
                                 ITimeProvider timeProvider,
                                 IContentfulFactory<ContentfulGroup, Group> groupFactory,
                                 IContentfulFactory<List<ContentfulGroup>, List<Group>> groupListFactory,
                                 IContentfulFactory<List<ContentfulGroupCategory>, List<GroupCategory>> groupCategoryListFactory,
                                 EventRepository eventRepository,
                                 ICache cache

            )
        {
            _dateComparer = new DateComparer(timeProvider);
            _client = clientManager.GetClient(config);
            _groupFactory = groupFactory;
            _groupListFactory = groupListFactory;
            _groupCategoryListFactory = groupCategoryListFactory;
            _eventRepository = eventRepository;
            _cache = cache;
        }

        public async Task<HttpResponse> GetGroup(string slug)
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No group found for '{slug}'");

            var group = _groupFactory.ToModel(entry);
            group.SetEvents(await _eventRepository.GetLinkedEvents<Group>(slug));

            return HttpResponse.Successful(group);
        }

        public async Task<HttpResponse> GetGroupResults(string category, double latitude, double longitude, string order, string location)
        {
            var groupResults = new GroupResults();

            var builder =
                new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
                    .Include(1)
                    .Limit(ContentfulQueryValues.LIMIT_MAX);

            if (longitude != 0 && latitude != 0) builder = builder.FieldEquals("fields.mapPosition[near]", latitude + "," + longitude + (location.ToLower() == Defaults.Groups.Location ? ",10" : ",3.2"));

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

            List<GroupCategory> groupCategoryResults = await GetGroupCategories();

            if (!string.IsNullOrEmpty(category) && !groupCategoryResults.Any(g => g.Slug.ToLower() == category.ToLower()))
                return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories found");

            groupResults.Categories = groupCategoryResults;

            return HttpResponse.Successful(groupResults);
        }

        public async Task<HttpResponse> GetAdministratorsGroups(string email)
        {
            var builder =
                new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldExists("fields.groupAdministrators")
                    .Include(1)
                    .Limit(ContentfulQueryValues.LIMIT_MAX);

            var groups = await _client.GetEntriesAsync(builder);

            groups = groups.Where(g => g.GroupAdministrators.Items.Any(i => i.Email == email));

            var result = _groupListFactory.ToModel(groups.ToList());

            return HttpResponse.Successful(result);
        }

        public async Task<List<GroupCategory>> GetGroupCategories()
        {
            return await _cache.GetFromCacheOrDirectlyAsync("group-categories", GetGroupCategoriesDirect);
        }

        private async Task<List<GroupCategory>> GetGroupCategoriesDirect()
        {
            var groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
            var groupCategoryEntries = await _client.GetEntriesAsync(groupCategoryBuilder);
            return _groupCategoryListFactory.ToModel(groupCategoryEntries.ToList()).ToList();
        }
    }
}
