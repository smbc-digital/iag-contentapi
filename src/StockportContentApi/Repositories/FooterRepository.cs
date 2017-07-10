using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http.Extensions;
using StockportContentApi.Client;
using StockportContentApi.Config;
using StockportContentApi.ContentfulFactories;
using StockportContentApi.ContentfulModels;
using StockportContentApi.Factories;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;

namespace StockportContentApi.Repositories
{
    public class FooterRepository
    {
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly string _contentType = "footer";
        private readonly IContentfulFactory<ContentfulFooter, Footer> _contentfulFactory;

        public FooterRepository(ContentfulConfig config, IContentfulClientManager clientManager, IContentfulFactory<ContentfulFooter, Footer> contentfulFactory)
        {
            _client = clientManager.GetClient(config);
            _contentfulFactory = contentfulFactory;
        }

        public async Task<HttpResponse> GetFooter() {

            var builder = new QueryBuilder<ContentfulFooter>().ContentTypeIs("footer").Include(1);

            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            var footer = _contentfulFactory.ToModel(entry);
            if (footer == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No footer found");

            return HttpResponse.Successful(footer);
        }
    }
}
