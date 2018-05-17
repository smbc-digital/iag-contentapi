using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();

            var privacyNotice = _contentfulFactory.ToModel(entry);

            return privacyNotice;
        }

        public async Task<List<PrivacyNotice>> GetAllPrivacyNotices()
        {
            var builder = new QueryBuilder<ContentfulPrivacyNotice>().ContentTypeIs("privacyNotice").Limit(1000);

            var entries = await _client.GetEntriesAsync(builder);

            var convertedEntries = entries.Select(entry => _contentfulFactory.ToModel(entry)).ToList();

            return convertedEntries;
        }
    }
}
