﻿namespace StockportContentApi.Repositories;

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

        QueryBuilder<ContentfulArticleForSiteMap> builder = new QueryBuilder<ContentfulArticleForSiteMap>().ContentTypeIs("article").Include(2).Limit(ContentfulQueryValues.LIMIT_MAX);
        ContentfulCollection<ContentfulArticleForSiteMap> articles = await _client.GetEntries(builder);

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
        QueryBuilder<ContentfulSection> builder = new QueryBuilder<ContentfulSection>().ContentTypeIs("section").FieldEquals("fields.slug", slug).Include(3);

        ContentfulCollection<ContentfulSection> entries = await _client.GetEntries(builder);

        ContentfulSection entry = entries.FirstOrDefault();
        Section Section = _contentfulFactory.ToModel(entry);

        return Section.GetType() == typeof(NullHomepage)
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Section found")
            : HttpResponse.Successful(Section);
    }
}