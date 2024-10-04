namespace StockportContentApi.Repositories;

public class LandingPageRepository : BaseRepository
{
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulLandingPage, LandingPage> _contentfulFactory;
    private readonly EventRepository _eventRepository;
    private readonly NewsRepository _newsRepository;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory;

    public LandingPageRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulLandingPage, LandingPage> contentfulFactory,
        IContentfulClientManager contentfulClientManager,
        EventRepository eventRepository,
        NewsRepository newsRepository,
        IContentfulFactory<ContentfulProfile, Profile> profileFactory)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
        _eventRepository = eventRepository;
        _newsRepository = newsRepository;
        _profileFactory = profileFactory;
    }

    public async Task<HttpResponse> GetLandingPage(string slug)
    {
        QueryBuilder<ContentfulLandingPage> builder = new QueryBuilder<ContentfulLandingPage>()
            .ContentTypeIs("landingPage").FieldEquals("fields.slug", slug).Include(2);
        ContentfulCollection<ContentfulLandingPage> entries = await _client.GetEntries(builder);
        ContentfulLandingPage entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No Landing Page found");

        LandingPage landingPage = _contentfulFactory.ToModel(entry);

        if (landingPage is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"Landing page not found {slug}");

        if (landingPage.PageSections is not null && landingPage.PageSections.Any())
        {
            foreach (ContentBlock contentBlock in landingPage.PageSections.Where(contentBlock => !string.IsNullOrEmpty(contentBlock.ContentType)))
            {
                switch (contentBlock.ContentType)
                {
                    case "NewsBanner" when !string.IsNullOrEmpty(contentBlock.AssociatedTagCategory):
                        {
                            News latestNewsResponse = await _newsRepository.GetLatestNewsByCategory(contentBlock.AssociatedTagCategory);

                            if (latestNewsResponse is not null)
                            {
                                contentBlock.NewsArticle = latestNewsResponse;
                                contentBlock.UseTag = false;
                            }
                            else
                            {
                                latestNewsResponse =
                                    await _newsRepository.GetLatestNewsByTag(contentBlock.AssociatedTagCategory);
                                if (latestNewsResponse is not null)
                                {
                                    contentBlock.NewsArticle = latestNewsResponse;
                                    contentBlock.UseTag = true;
                                }
                            }
                            break;
                        }
                    case "EventCards" when !string.IsNullOrEmpty(contentBlock.AssociatedTagCategory):
                        {
                            List<Event> events = await _eventRepository.GetEventsByCategory(contentBlock.AssociatedTagCategory, true);

                            if (!events.Any())
                                events = await _eventRepository.GetEventsByTag(contentBlock.AssociatedTagCategory, true);

                            contentBlock.Events = events.Take(3).ToList();
                            break;
                        }
                    case "ProfileBanner" when contentBlock.SubItems?.Any() is true:
                        {
                            ContentfulProfile profile = await GetProfile(contentBlock.SubItems.FirstOrDefault().Slug);

                            if (profile is not null)
                                contentBlock.Profile = _profileFactory.ToModel(profile);
                            break;
                        }
                    default:
                        break;
                }
            }
        }

        return HttpResponse.Successful(landingPage);
    }

    internal async Task<ContentfulProfile> GetProfile(string slug)
    {
        QueryBuilder<ContentfulProfile> profileBuilder = new QueryBuilder<ContentfulProfile>().ContentTypeIs("profile").FieldMatches(p => p.Slug, slug).Include(1);
        ContentfulCollection<ContentfulProfile> profileEntries = await _client.GetEntries(profileBuilder);

        return profileEntries.FirstOrDefault();
    }
}