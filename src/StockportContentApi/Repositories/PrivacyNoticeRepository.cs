using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;

namespace StockportContentApi.Repositories
{
    public interface IPrivacyNoticeRepository
    {
        Task<PrivacyNotice> GetPrivacyNotice(string slug);
        Task<List<PrivacyNotice>> GetAllPrivacyNotices();
    }

    public class PrivacyNoticeRepository : IPrivacyNoticeRepository
    {
        private readonly IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public PrivacyNoticeRepository(ContentfulConfig config, IContentfulFactory<ContentfulPrivacyNotice, PrivacyNotice> contentfulFactory,
            IContentfulClientManager contentfulClientManager)
        {
            _contentfulFactory = contentfulFactory;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<PrivacyNotice> GetPrivacyNotice(string slug)
        {
            var builder = new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").FieldEquals("fields.slug", slug).Include(3);

            var entries = await _client.GetEntries(builder);

            var entry = entries.FirstOrDefault();

            return entry is not null ? _contentfulFactory.ToModel(entry) : null;
        }

        public async Task<List<PrivacyNotice>> GetAllPrivacyNotices()
        {
            var builder = new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").Include(6);

            var entries = await GetAllEntries<ContentfulPrivacyNotice>("privacyNotice", builder);

            var convertedEntries = entries.Select(entry => _contentfulFactory.ToModel(entry)).ToList();

            return convertedEntries;
        }

        private async Task<IEnumerable<T>> GetAllEntries<T>(string contentType, QueryBuilder<T> builder)
        {
            var entries = await _client.GetEntries(builder.Limit(300).Skip(0));

            return entries.Items;
        }

    }
}
