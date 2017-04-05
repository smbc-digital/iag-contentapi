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

        public GroupRepository(ContentfulConfig config, IContentfulClientManager clientManager, 
                                 IContentfulFactory<ContentfulGroup, Group> groupFactory)
        {
            _client = clientManager.GetClient(config);
            _groupFactory = groupFactory;
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

        public async Task<HttpResponse> GetGroupResults()
        {
            var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            if (entries == null || !entries.Any()) return HttpResponse.Failure(HttpStatusCode.NotFound, "No groups found");

            var groups = GetAllGroups(entries).ToList();

            var GroupResults = new GroupResults() {Groups = groups};
            
            return HttpResponse.Successful(GroupResults);
        }

        private IEnumerable<Group> GetAllGroups(IEnumerable<ContentfulGroup> contentfulGroups)
        {
            var groupList = new List<Group>();
            foreach (var group in contentfulGroups)
            {
                var groupItem = _groupFactory.ToModel(group);
                groupList.Add(groupItem);
            }
            return groupList;
        }
    }
}
