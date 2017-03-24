using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Factories;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
namespace StockportContentApi.Repositories
{
    public class AtoZRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly ContentfulClient _contentfulClient;
        private readonly IFactory<AtoZ> _factory;
        private readonly UrlBuilder _urlBuilder;

        public AtoZRepository(ContentfulConfig config, IHttpClient httpClient, IFactory<AtoZ> factory)
        {
            _contentfulClient = new ContentfulClient(httpClient);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _factory = factory;
            _urlBuilder = new UrlBuilder(_contentfulApiUrl);
        }

        public async Task<HttpResponse> Get(string letter)
        {
            var atozItems = new List<AtoZ>();

            atozItems.AddRange(await GetAtoZItemFromContentType("article", letter));
            atozItems.AddRange(await GetAtoZItemFromContentType("topic", letter));

            atozItems = atozItems.OrderBy(o => o.Title).ToList();

            return !atozItems.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No results found")
                : HttpResponse.Successful(atozItems);
        }

        private async Task<List<AtoZ>> GetAtoZItemFromContentType(string contentType, string letter)
        {
            var atozList = new List<AtoZ>();
            var contentfulResponse = await _contentfulClient.Get(_urlBuilder.UrlFor(type:contentType,displayOnAtoZ:true));

            foreach (var item in contentfulResponse.Items)
            {
                AtoZ buildItem = _factory.Build(item, contentfulResponse);

                var matchingItems = buildItem.SetTitleStartingWithLetter(letter);
                atozList.AddRange(matchingItems);
            }

            return atozList;
        }
    }
}
