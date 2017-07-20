using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using Contentful.Core.Search;
using StockportContentApi.ContentfulModels;

namespace StockportContentApi.Repositories
{
    public class StartPageRepository
    {
        private readonly IFactory<StartPage> _startPageFactory;
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly UrlBuilder _urlBuilder;

        public StartPageRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<StartPage> startPageFactory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _startPageFactory = startPageFactory;
            _urlBuilder = new UrlBuilder(_contentfulApiUrl);
        }

        public async Task<HttpResponse> GetStartPage(string startPageSlug)
        {
            var contentfulResponse = await _contentfulClient.Get(_urlBuilder.UrlFor(type:"startPage", referenceLevel:1, slug:startPageSlug));

            if (!contentfulResponse.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'");

            var startPageEntry = contentfulResponse.GetFirstItem();
            var startPage =  _startPageFactory.Build(startPageEntry, contentfulResponse);

            return startPage.GetType() == typeof(NullStartPage) ?
                HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'") : 
                HttpResponse.Successful(startPage);
        }

        public async Task<HttpResponse> Get()
        {
            var contentfulResponse = await _contentfulClient.Get(_urlBuilder.UrlFor(type: "startPage", referenceLevel: 1));

            if (!contentfulResponse.HasItems())
                return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found");

            var startpage = GetAllStartPages(contentfulResponse);
          
            return startpage == null || !contentfulResponse.Items.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Topics found")
                : HttpResponse.Successful(startpage);
        }

        private IEnumerable<StartPage> GetAllStartPages(ContentfulResponse entries)
        {
            var entriesList = new List<StartPage>();
            foreach (var entry in entries.Items)
            {                
                var startPageItem = _startPageFactory.Build(entry, entries);
                entriesList.Add(startPageItem);
            }

            return entriesList;
        }
    }
}
