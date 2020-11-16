using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

            var privacyNotice = _contentfulFactory.ToModel(entry);

            return privacyNotice;
        }

        public async Task<List<PrivacyNotice>> GetAllPrivacyNotices()
        {
            var builder = new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").Include(6);

            var entries = GetAllEntries<ContentfulPrivacyNotice>("privacyNotice", builder);

            var convertedEntries = entries.Select(entry => _contentfulFactory.ToModel(entry)).ToList();

            return convertedEntries;
        }

        private List<T> GetAllEntries<T>(string contentType, QueryBuilder<T> builder)
        {
            var result = new List<T>();

            var builderString = builder.Limit(50).Skip(49).Build();
            builderString = builderString.Replace("skip=49", "skip=xx");

            var totalItems = 0;
            var skip = 0;

            do
            {
                builderString = builderString.Replace("skip=xx", $"skip={skip}");

                var entries = _client.GetEntries<T>(builderString).Result;

                builderString = builderString.Replace($"skip={skip}", "skip=xx");

                result = result.Concat(entries.Items).ToList();

                totalItems = entries.Total;
                skip += 50;
            } while (result.Count() < totalItems);

            return result;
        }

    }
}
