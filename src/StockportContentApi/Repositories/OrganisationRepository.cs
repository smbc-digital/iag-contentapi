using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using System.Collections.Generic;

namespace StockportContentApi.Repositories
{
    public class OrganisationRepository
    {
        private readonly IContentfulFactory<ContentfulOrganisation, Organisation> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public OrganisationRepository(ContentfulConfig config,
            IContentfulFactory<ContentfulOrganisation, Organisation> contentfulFactory,
            IContentfulClientManager contentfulClientManager)
        {
            _contentfulFactory = contentfulFactory;
            _client = contentfulClientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetOrganisation(string slug)
        {
            var builder = new QueryBuilder<ContentfulOrganisation>().ContentTypeIs("organisation").FieldEquals("fields.slug", slug);

            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();
            var organisation = _contentfulFactory.ToModel(entry);

            return organisation == null
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Organisation found")
                : HttpResponse.Successful(organisation);
        }
    }
}