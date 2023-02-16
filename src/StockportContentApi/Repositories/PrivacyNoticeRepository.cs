using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using StockportContentApi.Utils;


namespace StockportContentApi.Repositories
{
    public interface IPrivacyNoticeRepository
    {
        Task<PrivacyNotice> GetPrivacyNotice(string slug);
        Task<List<PrivacyNotice>> GetAllPrivacyNotices();
        Task<List<PrivacyNotice>> GetPrivacyNoticesByTitle(string title);
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
            var builder = new QueryBuilder<ContentfulPrivacyNotice>()
                .ContentTypeIs("privacyNotice")
                .FieldEquals("fields.slug", slug)
                .Include(2);

            var entries = await _client.GetEntries(builder);
            var entry = entries.FirstOrDefault();

            return entry is not null ? _contentfulFactory.ToModel(entry) : null;
        }

        public async Task<List<PrivacyNotice>> GetAllPrivacyNotices()
        {
            var builder = new QueryBuilder<ContentfulPrivacyNotice>()
                .ContentTypeIs("privacyNotice")
                .Include(2);

            var entries = await GetAllEntries<ContentfulPrivacyNotice>(builder);

            var convertedEntries = entries.Select(entry => _contentfulFactory.ToModel(entry)).ToList();

            return convertedEntries;
        }

        public async Task<List<PrivacyNotice>> GetPrivacyNoticesByTitle(string title)
        {
            var builder = new QueryBuilder<ContentfulPrivacyNotice>()
                .ContentTypeIs("privacyNotice")
                .Include(2);

            var entries = await GetAllEntries<ContentfulPrivacyNotice>(builder);

            var convertedEntries = entries.Select(entry => _contentfulFactory.ToModel(entry)).ToList();

            return convertedEntries;
        }

        private async Task<IEnumerable<T>> GetAllEntries<T>(QueryBuilder<T> builder)
        {
            var entries = await _client.GetEntries(builder.Limit(ContentfulQueryValues.LIMIT_MAX));
            return entries.Items;
        }
    }
}