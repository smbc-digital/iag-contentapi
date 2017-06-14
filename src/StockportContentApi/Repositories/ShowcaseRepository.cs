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
    public class ShowcaseRepository
    {
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulShowcase, Showcase> _contentfulFactory;
        private readonly IContentfulFactory<List<ContentfulEvent>, List<Event>> _eventListFactory;
        private readonly IContentfulFactory<ContentfulNews, News> _newsFactory;
        private readonly Contentful.Core.IContentfulClient _client;
        private readonly EventRepository _eventRepository;

        public ShowcaseRepository(ContentfulConfig config,
            IContentfulFactory<ContentfulShowcase, Showcase> showcaseBuilder,
            IContentfulClientManager contentfulClientManager,
            IContentfulFactory<List<ContentfulEvent>, List<Event>> eventListBuilder,
            IContentfulFactory<ContentfulNews, News> newsBuilder,
            ITimeProvider timeProvider, EventRepository eventRepository)
        {
            _dateComparer = new DateComparer(timeProvider);
            _contentfulFactory = showcaseBuilder;
            _eventListFactory = eventListBuilder;
            _newsFactory = newsBuilder;
            _client = contentfulClientManager.GetClient(config);
            _eventRepository = eventRepository;
        }

        public async Task<HttpResponse> GetShowcases(string slug)
        {
            var builder = new QueryBuilder<ContentfulShowcase>().ContentTypeIs("showcase").FieldEquals("fields.slug", slug).Include(3);


            var entries = await _client.GetEntriesAsync(builder);

            var entry = entries.FirstOrDefault();
            var showcase = _contentfulFactory.ToModel(entry);

            showcase.Events = await _eventRepository.GetEventsByCategory(showcase.EventCategory);

            var news = await PopulateNews(showcase.NewsCategoryTag);
            if (news != null)
            {
                showcase.NewsArticle = news.News;
                showcase.NewsCategoryOrTag = news.Type;
            }

            return showcase.GetType() == typeof(NullHomepage)
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Showcase found")
                : HttpResponse.Successful(showcase);
        }

        private async Task<ShowcaseNews> PopulateNews(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
            {
                return null;
            }

            News result = null;
            var type = string.Empty;

            var newsBuilder =
                new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                    .FieldMatches(n => n.Categories, tag)
                    .Include(1);
            var newsEntry = await _client.GetEntriesAsync(newsBuilder);

            if (newsEntry != null && newsEntry.Any())
            {
                type = "C";
            }
            else
            {
                newsBuilder =
                    new QueryBuilder<ContentfulNews>().ContentTypeIs("news")
                        .FieldMatches(n => n.Tags, tag)
                        .Include(1);
                newsEntry = await _client.GetEntriesAsync(newsBuilder);

                if (newsEntry != null && newsEntry.Any())
                {
                    type = "T";
                }
            }

            if (newsEntry != null && newsEntry.Any())
            {
                var now = DateTime.Now.Date;
                var article = newsEntry.Where(e => now > e.SunriseDate.Date)
                                        .Where(e => now < e.SunsetDate.Date)
                                        .OrderByDescending(n => n.SunriseDate)
                                        .Take(1)
                                        .FirstOrDefault();

                if (article != null)
                {
                    var newsArticle = _newsFactory.ToModel(article);
                    result = newsArticle;
                }
            }

            return new ShowcaseNews() { News = result, Type = type };
        }
    }
}