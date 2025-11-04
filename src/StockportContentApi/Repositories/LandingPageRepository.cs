namespace StockportContentApi.Repositories;

public interface ILandingPageRepository
{
    Task<HttpResponse> GetLandingPage(string slug, string tagId);
}

public class LandingPageRepository(
        ContentfulConfig config,
        IContentfulFactory<ContentfulLandingPage, LandingPage> contentfulFactory,
        IContentfulClientManager contentfulClientManager,
        EventRepository eventRepository,
        NewsRepository newsRepository,
        IContentfulFactory<ContentfulProfile, Profile> profileFactory) : BaseRepository, ILandingPageRepository
{
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulLandingPage, LandingPage> _contentfulFactory = contentfulFactory;
    private readonly EventRepository _eventRepository = eventRepository;
    private readonly NewsRepository _newsRepository = newsRepository;
    private readonly IContentfulFactory<ContentfulProfile, Profile> _profileFactory = profileFactory;

    public async Task<HttpResponse> GetLandingPage(string slug, string tagId)
    {
        QueryBuilder<ContentfulLandingPage> builder = new QueryBuilder<ContentfulLandingPage>()
            .ContentTypeIs("landingPage")
            .FieldEquals("fields.slug", slug)
            .Include(2);
        
        ContentfulCollection<ContentfulLandingPage> entries = await _client.GetEntries(builder);
        ContentfulLandingPage entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No landing page found");

        LandingPage landingPage = _contentfulFactory.ToModel(entry);

        if (landingPage is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"Landing page not found {slug}");
        
        await ProcessLandingPageContent(landingPage, tagId);

        return HttpResponse.Successful(landingPage);
    }

    private async Task ProcessLandingPageContent(LandingPage landingPage, string tagId)
    {
        if (landingPage.PageSections is null || !landingPage.PageSections.Any()) return;

        foreach (ContentBlock contentBlock in landingPage.PageSections.Where(contentBlock => !string.IsNullOrEmpty(contentBlock.ContentType)))
        {
            switch (contentBlock.ContentType)
            {
                case "NewsBanner":
                    await PopulateNewsContent(contentBlock, tagId);
                    break;

                case "NewsCards":
                    await PopulateNewsCardsContent(contentBlock, tagId);
                    break;
                
                case "EventCards":
                    await PopulateEventContent(contentBlock, tagId);
                    break;

                case "ProfileBanner":
                    await PopulateProfileContent(contentBlock, tagId);
                    break;

                default:
                    break;
            }
        }
    }

    private async Task PopulateNewsContent(ContentBlock contentBlock, string tagId)
    {
        if (contentBlock.SubItems?.Any() is true)
        {
            ContentBlock firstSubItem = contentBlock.SubItems.FirstOrDefault();
            HttpResponse newsResponse = await _newsRepository.GetNews(firstSubItem.Slug, tagId);
            News newsSubItem = newsResponse.Get<News>();
            
            if (newsSubItem is not null)
            {
                contentBlock.NewsArticle = newsSubItem;
                contentBlock.UseTag = false;
                return;
            }
        }

        if (string.IsNullOrEmpty(contentBlock.AssociatedTagCategory))
        {
            HttpResponse latestNewsResponse = await _newsRepository.GetNewsByLimit(1, tagId);
            List<News> latestNews = latestNewsResponse.Get<List<News>>();
            if (latestNews is not null && latestNews.Any())
            {
                contentBlock.NewsArticle = latestNews.First();
                contentBlock.UseTag = false;
            }
        }

        IEnumerable<string> tagsOrCategories = contentBlock.AssociatedTagCategory.Split(',').Select(tag => tag.Trim());
        foreach (string tagOrCategory in tagsOrCategories)
        {
            List<News> news = await _newsRepository.GetLatestNewsByCategory(tagOrCategory, tagId)
                        ?? await _newsRepository.GetLatestNewsByTag(tagOrCategory, tagId);

            if (news is not null)
            {
                contentBlock.NewsArticle = news.FirstOrDefault();
                contentBlock.UseTag = news.First().Slug.Equals((await _newsRepository.GetLatestNewsByTag(tagOrCategory, tagId))?.First().Slug);
                break;
            }
        }
    }
    
    private async Task PopulateEventContent(ContentBlock contentBlock, string tagId)
    {
        if (string.IsNullOrEmpty(contentBlock.AssociatedTagCategory))
        {
            HttpResponse upcomingEventsResponse = await _eventRepository.GetUpcomingEvents(tagId, 3);
            List<Event> upcomingEvents = upcomingEventsResponse.Get<List<Event>>();

            if (upcomingEvents is not null && upcomingEvents.Any())
            {
                contentBlock.Events = upcomingEvents;
                return;
            }
        }

        IEnumerable<string> tagsOrCategories = contentBlock.AssociatedTagCategory.Split(',').Select(tag => tag.Trim());
        List<Event> events = new();

        IEnumerable<Task<List<Event>>> tasks = tagsOrCategories.Select(async tagOrCategory =>
        {
            List<Event> categoryEvents = await _eventRepository.GetEventsByCategory(tagOrCategory, true, tagId);
            return categoryEvents.Any()
                ? categoryEvents
                : await _eventRepository.GetEventsByTag(tagOrCategory, true, tagId);
        });

        foreach (List<Event> task in await Task.WhenAll(tasks))
        {
            events.AddRange(task);
        }

        contentBlock.Events = events
            .GroupBy(evnt => evnt.Slug)
            .Select(g => g.First())
            .OrderBy(evnt => evnt.EventDate)
            .ThenBy(evnt => TimeSpan.Parse(evnt.StartTime))
            .ThenBy(evnt => evnt.Title)
            .Take(3)
            .ToList();
    }

    private async Task PopulateProfileContent(ContentBlock contentBlock, string tagId)
    {
        if (contentBlock.SubItems?.Any() is false) return;

        ContentBlock firstSubItem = contentBlock.SubItems.First();
        ContentfulProfile profile = await GetProfile(firstSubItem.Slug, tagId);

        if (profile is not null)
            contentBlock.Profile = _profileFactory.ToModel(profile);
    }

    private async Task PopulateNewsCardsContent(ContentBlock contentBlock, string tagId, int quantity = 3)
    {
        if (string.IsNullOrEmpty(contentBlock.AssociatedTagCategory))
        {
            HttpResponse latestNewsResponse = await _newsRepository.GetNewsByLimit(quantity, tagId);
            List<News> upcomingNews = latestNewsResponse.Get<List<News>>();

            if (upcomingNews is not null && upcomingNews.Any())
            {
                contentBlock.News = upcomingNews;
                contentBlock.IsLatest = true;
                return;
            }
        }

        IEnumerable<string> tagsOrCategories = contentBlock.AssociatedTagCategory.Split(',').Select(tag => tag.Trim());
        List<News> news = new();

        foreach (string tagOrCategory in tagsOrCategories)
        {
            news = await _newsRepository.GetLatestNewsByCategory(tagOrCategory, tagId, quantity);

            if (news is null)
            {
                news = await _newsRepository.GetLatestNewsByTag(tagOrCategory, tagId, quantity);
                contentBlock.UseTag = true;
            }

            if (news is not null)
            {
                contentBlock.News = news;
                contentBlock.IsLatest = false;
                break;
            }
        }
    }

    internal async Task<ContentfulProfile> GetProfile(string slug, string tagId)
    {
        QueryBuilder<ContentfulProfile> profileBuilder = new QueryBuilder<ContentfulProfile>()
            .ContentTypeIs("profile")
            .FieldMatches(p => p.Slug, slug)
            .Include(1);
        ContentfulCollection<ContentfulProfile> profileEntries = await _client.GetEntries(profileBuilder);

        return profileEntries.FirstOrDefault();
    }
}