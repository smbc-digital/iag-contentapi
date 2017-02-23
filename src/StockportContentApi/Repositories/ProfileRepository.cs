﻿using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using System.Linq;
using StockportContentApi.Client;

namespace StockportContentApi.Repositories
{
    public class ProfileRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;

        public ProfileRepository(ContentfulConfig config, IContentfulClientManager clientManager, 
                                 IContentfulFactory<ContentfulProfile, Profile> profileFactory)
        {
            _client = clientManager.GetClient(config);
            _profileFactory = profileFactory;
        }

        public async Task<HttpResponse> GetProfile(string slug)
        {
            var builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            return entry == null 
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No profile found for '{slug}'") 
                : HttpResponse.Successful(_profileFactory.ToModel(entry));
        }
    }
}
