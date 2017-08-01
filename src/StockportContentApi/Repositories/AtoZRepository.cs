using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using StockportContentApi.Config;
using StockportContentApi.Http;
using StockportContentApi.Model;
using StockportContentApi.Utils;
using System;
using Contentful.Core.Search;
using StockportContentApi.Client;
using StockportContentApi.ContentfulModels;
using StockportContentApi.ContentfulFactories;

namespace StockportContentApi.Repositories
{
    public class AtoZRepository
    {
        private readonly string _contentfulApiUrl;
        private readonly DateComparer _dateComparer;
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly IContentfulFactory<ContentfulAtoZ, AtoZ> _contentfulAtoZFactory;

        public AtoZRepository(ContentfulConfig config, IContentfulClientManager clientManager, 
            IContentfulFactory<ContentfulAtoZ, AtoZ> contentfulAtoZFactory,
            ITimeProvider timeProvider)
        {
            _client = clientManager.GetClient(config);
            _contentfulApiUrl = config.ContentfulUrl.ToString();
            _contentfulAtoZFactory = contentfulAtoZFactory;
           _dateComparer = new DateComparer(timeProvider);
        }

        public async Task<HttpResponse> Get(string letter)
        {
            var atozItems = new List<AtoZ>();
            atozItems.AddRange(await GetAtoZItemFromContentType("article", letter));
            atozItems.AddRange(await GetAtoZItemFromContentType("topic", letter));
            atozItems.AddRange(await GetAtoZItemFromContentType("showcase", letter));

            atozItems = atozItems.OrderBy(o => o.Title).ToList();

            return !atozItems.Any()
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No results found")
                : HttpResponse.Successful(atozItems);
        }

        private async Task<List<AtoZ>> GetAtoZItemFromContentType(string contentType, string letter)
        {
            var atozList = new List<AtoZ>();           
            var builder = new QueryBuilder<ContentfulAtoZ>().ContentTypeIs(contentType).Include(2);
            var entries = await _client.GetEntriesAsync(builder);
            var entriesWithDisplayOn = entries != null ? entries.Where(x => x.DisplayOnAZ == "True") : null;

            if (entriesWithDisplayOn != null)
            {
                foreach (var item in entriesWithDisplayOn)
                {
                    DateTime sunriseDate = DateComparer.DateFieldToDate(item.SunriseDate);
                    DateTime sunsetDate = DateComparer.DateFieldToDate(item.SunsetDate);
                    if (_dateComparer.DateNowIsWithinSunriseAndSunsetDates(sunriseDate, sunsetDate))
                    {
                        AtoZ buildItem = _contentfulAtoZFactory.ToModel(item);
                        var matchingItems = buildItem.SetTitleStartingWithLetter(letter);
                        atozList.AddRange(matchingItems);
                    }
                }
            }
            
            return atozList;
        }
    }
}
