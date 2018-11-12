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
    public class SectionRepository
    {
        private readonly DateComparer _dateComparer;
        private readonly IContentfulFactory<ContentfulSection, Section> _contentfulFactory;
        private readonly Contentful.Core.IContentfulClient _client;

        public SectionRepository(ContentfulConfig config,
            IContentfulFactory<ContentfulSection, Section> SectionBuilder,
            IContentfulClientManager contentfulClientManager,
            ITimeProvider timeProvider)
        {
            _dateComparer = new DateComparer(timeProvider);
            _contentfulFactory = SectionBuilder;
            _client = contentfulClientManager.GetClient(config);
        }
        public async Task<HttpResponse> Get()
        {
            var sections = new List<ContentfulSectionForSiteMap>();

            var builder = new QueryBuilder<ContentfulArticleForSiteMap>().ContentTypeIs("article").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX);
            var articles = await _client.GetEntries(builder);

            foreach (var article in articles.Where(e => e.Sections.Any()))
            {
                foreach (var section in article.Sections)
                {
                    sections.Add(new ContentfulSectionForSiteMap { Slug = $"{article.Slug}/{section.Slug}", SunriseDate = section.SunriseDate, SunsetDate = section.SunsetDate });
                }
            }

            return sections.GetType() == typeof(NullHomepage)
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Sections found")
                : HttpResponse.Successful(sections);
        }

        public async Task<HttpResponse> GetSections(string slug)
        {
            var builder = new QueryBuilder<ContentfulSection>().ContentTypeIs("section").FieldEquals("fields.slug", slug).Include(3);

            var entries = await _client.GetEntries(builder);

            var entry = entries.FirstOrDefault();
            var Section = _contentfulFactory.ToModel(entry);

            return Section.GetType() == typeof(NullHomepage)
                ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Section found")
                : HttpResponse.Successful(Section);
        }
    }
}