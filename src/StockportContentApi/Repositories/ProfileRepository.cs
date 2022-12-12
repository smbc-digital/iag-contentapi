using System.Net;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public interface IProfileRepository
    {
        Task<HttpResponse> GetProfile(string slug);
        Task<HttpResponse> Get();
    }

    public class ProfileRepository : IProfileRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;

        public ProfileRepository()
        {

        }

        public ProfileRepository(ContentfulConfig config, IContentfulClientManager clientManager,
                                 IContentfulFactory<ContentfulProfile, Profile> profileFactory)
        {
            _client = clientManager.GetClient(config);
            _profileFactory = profileFactory;
        }

        public async Task<HttpResponse> GetProfile(string slug)
        {
            var builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldEquals("fields.slug", slug).Include(1);
            var entries = await _client.GetEntries(builder);
            var entry = entries.FirstOrDefault();

            return entry == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No profile found for '{slug}'")
                : HttpResponse.Successful(_profileFactory.ToModel(entry));
        }

        public async Task<HttpResponse> Get()
        {
            var builder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").Include(1);
            var entries = await _client.GetEntries(builder);

            if (entries == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No profiles found");

            var models = entries.Select(e => _profileFactory.ToModel(e));

            return HttpResponse.Successful(models);
        }
    }
}
