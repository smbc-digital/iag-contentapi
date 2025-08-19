namespace StockportContentApi.Repositories;

public interface ISectionRepository
{
    Task<HttpResponse> Get();
    Task<HttpResponse> GetSections(string slug);
}

public class SectionRepository(ContentfulConfig config,
                            IContentfulFactory<ContentfulSection, Section> SectionBuilder,
                            IContentfulClientManager contentfulClientManager) : ISectionRepository
{
    private readonly IContentfulFactory<ContentfulSection, Section> _contentfulFactory = SectionBuilder;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);

    public async Task<HttpResponse> Get()
    {
        List<ContentfulSectionForSiteMap> sections = new();

        QueryBuilder<ContentfulArticleForSiteMap> builder = new QueryBuilder<ContentfulArticleForSiteMap>()
                                                                .ContentTypeIs("article")
                                                                .Include(2)
                                                                .Limit(ContentfulQueryValues.LIMIT_MAX);

        ContentfulCollection<ContentfulArticleForSiteMap> articles = await _client.GetEntries(builder);

        sections.AddRange(articles.Where(e => e.Sections.Any()).SelectMany(article => article.Sections.Select(section => new ContentfulSectionForSiteMap
        {
            Slug = $"{article.Slug}/{section.Slug}",
            SunriseDate = section.SunriseDate,
            SunsetDate = section.SunsetDate
        })));
        
        return sections.GetType().Equals(typeof(NullHomepage))
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No sections found")
            : HttpResponse.Successful(sections);
    }

    public async Task<HttpResponse> GetSections(string slug)
    {
        QueryBuilder<ContentfulSection> builder = new QueryBuilder<ContentfulSection>()
                                                    .ContentTypeIs("section")
                                                    .FieldEquals("fields.slug", slug)
                                                    .Include(3);
        
        ContentfulCollection<ContentfulSection> entries = await _client.GetEntries(builder);
        ContentfulSection entry = entries.FirstOrDefault();

        Section section = entry is null
            ? null
            : _contentfulFactory.ToModel(entry);

        var contentfulEntry = new
        {
            sys = new
            {
                id = entry.Sys.Id,
                type = entry.Sys.Type,
                locale = entry.Sys.Locale,
                contentType = entry.Sys.ContentType,
                environment = entry.Sys.Environment,
                space = entry.Sys.Space
            },
            fields = new
            {
                title = entry.Title,
                body = entry.Body,
                alertsInline = entry.AlertsInline,
                inlineQuotes = entry.InlineQuotes,
                logoAreaTitle = entry.LogoAreaTitle,
                trustedLogos = entry.TrustedLogos,
                documents = entry.Documents,
                profiles = entry.Profiles
            }
        };

        section.RawContentful = JObject.FromObject(contentfulEntry);

        return section.GetType().Equals(typeof(NullHomepage))
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No section found")
            : HttpResponse.Successful(section);
    }
}