using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Contentful.Core.Search;
using Microsoft.AspNetCore.Http.Extensions;
using StockportContentApi.Client;
using StockportContentApi.Config;
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

        public FooterRepository(ContentfulConfig config, IContentfulClientManager clientManager)
        {
            _client = clientManager.GetClient(config);
        }

        public async Task<HttpResponse> GetFooter() {

            var builder = new QueryBuilder<Footer>().ContentTypeIs("footer").Include(1);

            var entries = await _client.GetEntriesAsync(builder);
            var entry = entries.FirstOrDefault();

            if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No footer found");

            return HttpResponse.Successful(entry);
        }
    }
}
