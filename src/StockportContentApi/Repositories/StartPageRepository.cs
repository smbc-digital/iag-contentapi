﻿namespace StockportContentApi.Repositories;

public class StartPageRepository
{
    private readonly IContentfulFactory<ContentfulStartPage, StartPage> _contentfulFactory;
    private readonly IContentfulClient _client;
    private readonly DateComparer _dateComparer;

    public StartPageRepository(ContentfulConfig config, IContentfulClientManager contentfulClientManager, IContentfulFactory<ContentfulStartPage, StartPage> contentfulFactory, ITimeProvider timeProvider)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
        _dateComparer = new DateComparer(timeProvider);
    }

    public async Task<HttpResponse> GetStartPage(string startPageSlug)
    {
        QueryBuilder<ContentfulStartPage> builder = new QueryBuilder<ContentfulStartPage>().ContentTypeIs("startPage").FieldEquals("fields.slug", startPageSlug).Include(3);
        ContentfulCollection<ContentfulStartPage> entries = await _client.GetEntries(builder);

        if (!entries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'");

        ContentfulStartPage startPageEntry = entries.FirstOrDefault();
        StartPage startPage = _contentfulFactory.ToModel(startPageEntry);

        if (!_dateComparer.DateNowIsWithinSunriseAndSunsetDates(startPage.SunriseDate, startPage.SunsetDate))
            startPage = new NullStartPage();

        return startPage.GetType().Equals(typeof(NullStartPage))
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found for '{startPageSlug}'")
            : HttpResponse.Successful(startPage);
    }

    public async Task<HttpResponse> Get()
    {
        QueryBuilder<ContentfulStartPage> builder = new QueryBuilder<ContentfulStartPage>().ContentTypeIs("startPage").Include(3);
        ContentfulCollection<ContentfulStartPage> entries = await _client.GetEntries(builder);

        if (!entries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No start page found");

        IEnumerable<StartPage> startPages = entries.Select(_contentfulFactory.ToModel).ToList()
                                                .Where(startPage => _dateComparer.DateNowIsWithinSunriseAndSunsetDates(startPage.SunriseDate, startPage.SunsetDate));

        return startPages is null || !startPages.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No start page found")
            : HttpResponse.Successful(startPages);
    }
}