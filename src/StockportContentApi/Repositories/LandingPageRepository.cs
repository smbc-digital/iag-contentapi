namespace StockportContentApi.Repositories;

public interface ILandingPageRepository
{
    Task<HttpResponse> GetLandingPage(string slug);
}

public class LandingPageRepository(ContentfulConfig config,
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
                            List<string> associatedTagsCategories = contentBlock.AssociatedTagCategory.Split(',').ToList();
                            News latestNewsResponse = null;

                            foreach (string tagOrCategory in associatedTagsCategories)
                            {
                                latestNewsResponse = await _newsRepository.GetLatestNewsByCategory(tagOrCategory.Trim());
                                if (latestNewsResponse is not null)
                                {
                                    contentBlock.NewsArticle = latestNewsResponse;
                                    contentBlock.UseTag = false;
                                    break;
                                }

                                latestNewsResponse = await _newsRepository.GetLatestNewsByTag(tagOrCategory.Trim());
                                if (latestNewsResponse is not null)
                                {
                                    contentBlock.NewsArticle = latestNewsResponse;
                                    contentBlock.UseTag = true;
                                    break;
                                }
                            }

                            break;
                        }
                    case "EventCards" when !string.IsNullOrEmpty(contentBlock.AssociatedTagCategory):
                        {
                            List<string> associatedTagsCategories = contentBlock.AssociatedTagCategory.Split(",").ToList();
                            List<Event> events = new();

                            foreach (string associatedTagCategory in associatedTagsCategories)
                            {
                                List<Event> categoryEvents = await _eventRepository.GetEventsByCategory(associatedTagCategory.Trim(), true);
                                if (categoryEvents.Any())
                                    events.AddRange(categoryEvents);
                                else
                                    events.AddRange(await _eventRepository.GetEventsByTag(associatedTagCategory.Trim(), true));
                            }

                            contentBlock.Events = events
                                                .GroupBy(evnt => evnt.Slug)
                                                .Select(evnt => evnt.First())
                                                .OrderBy(evnt => evnt.EventDate)
                                                .ThenBy(evnt => TimeSpan.Parse(evnt.StartTime))
                                                .ThenBy(evnt => evnt.Title)
                                                .Take(3).ToList();

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