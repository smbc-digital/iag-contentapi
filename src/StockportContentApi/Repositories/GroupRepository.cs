using StockportContentApi.Constants;

namespace StockportContentApi.Repositories;

public interface IGroupRepository
{
    Task<HttpResponse> Get();
    Task<ContentfulGroup> GetContentfulGroup(string slug);
    Task<HttpResponse> GetGroupHomepage();
    Task<HttpResponse> GetGroup(string slug, bool onlyActive);
    Task<List<Group>> GetLinkedGroups(Group group);
    Task<HttpResponse> GetGroupResults(GroupSearch groupSearch, string slugs);
    Task<HttpResponse> GetAdministratorsGroups(string email);
    Task<List<GroupCategory>> GetGroupCategories();
    Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategories();
    Task<List<Group>> GetLinkedGroupsByOrganisation(string slug);
}

public class GroupRepository : BaseRepository, IGroupRepository
{
    private readonly ICache _cache;
    private readonly IContentfulClient _client;
    private readonly IConfiguration _configuration;
    private readonly DateComparer _dateComparer;
    private readonly EventRepository _eventRepository;
    private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _groupCategoryFactory;
    private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;
    private readonly IContentfulFactory<ContentfulGroupHomepage, GroupHomepage> _groupHomepageContentfulFactory;
    private readonly int _groupsTimeout;

    public GroupRepository(ContentfulConfig config, IContentfulClientManager clientManager,
        ITimeProvider timeProvider,
        IContentfulFactory<ContentfulGroup, Group> groupFactory,
        IContentfulFactory<ContentfulGroupCategory, GroupCategory> groupCategoryFactory,
        IContentfulFactory<ContentfulGroupHomepage, GroupHomepage> groupHomepageContentfulFactory,
        EventRepository eventRepository,
        ICache cache,
        IConfiguration configuration
    )
    {
        _dateComparer = new(timeProvider);
        _client = clientManager.GetClient(config);
        _groupFactory = groupFactory;
        _groupCategoryFactory = groupCategoryFactory;
        _eventRepository = eventRepository;
        _cache = cache;
        _configuration = configuration;
        int.TryParse(_configuration["redisExpiryTimes:Groups"], out _groupsTimeout);
        _groupHomepageContentfulFactory = groupHomepageContentfulFactory;
    }

    public async Task<HttpResponse> Get()
    {
        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
        ContentfulCollection<ContentfulGroup> entries = await GetAllEntriesAsync(_client, builder);
        IEnumerable<ContentfulGroup> contentfulGroups = entries as IEnumerable<ContentfulGroup> ?? entries.ToList();

        contentfulGroups = contentfulGroups.Where(group => _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo));

        IEnumerable<Group> groupList = contentfulGroups.Select(group => _groupFactory.ToModel(group));

        return entries is null || !groupList.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Groups found")
            : HttpResponse.Successful(groupList.ToList());
    }

    public async Task<ContentfulGroup> GetContentfulGroup(string slug)
    {
        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);
        ContentfulCollection<ContentfulGroup> entries = await _client.GetEntries(builder);
        ContentfulGroup entry = entries.FirstOrDefault();

        return entry;
    }

    public async Task<HttpResponse> GetGroupHomepage()
    {
        QueryBuilder<ContentfulGroupHomepage> builder = new QueryBuilder<ContentfulGroupHomepage>().ContentTypeIs("groupHomepage").Include(1);
        ContentfulCollection<ContentfulGroupHomepage> entries = await _client.GetEntries(builder);
        ContentfulGroupHomepage entry = entries.ToList().First();

        return entry is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No event homepage found")
            : HttpResponse.Successful(_groupHomepageContentfulFactory.ToModel(entry));
    }

    public async Task<HttpResponse> GetGroup(string slug, bool onlyActive)
    {
        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group")
            .FieldEquals("fields.slug", slug).Include(1);

        ContentfulCollection<ContentfulGroup> entries = await _client.GetEntries(builder);

        ContentfulGroup entry = onlyActive
            ? entries.FirstOrDefault(group =>
                _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo))
            : entries.FirstOrDefault();

        if (entry is null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No group found for '{slug}'");

        Group group = _groupFactory.ToModel(entry);
        group.SetEvents(await _eventRepository.GetLinkedEvents<Group>(slug));

        string twitterUser = group.Twitter;
        string faceBookUser = group.Facebook;
        if (twitterUser is not null && twitterUser.StartsWith("@"))
        {
            twitterUser = twitterUser.Replace("@", "/");
            group.Twitter = $"https://www.twitter.com{twitterUser}";
        }

        if (!string.IsNullOrEmpty(faceBookUser) && faceBookUser.StartsWith("/"))
        {
            faceBookUser = faceBookUser.Replace("/", string.Empty);
            group.Facebook = $"https://www.facebook.co.uk{faceBookUser}";
        }

        if (!string.IsNullOrEmpty(faceBookUser) &&
            (!faceBookUser.StartsWith("http") || !faceBookUser.StartsWith("http")))
            group.Facebook = $"https://{faceBookUser}";

        if (group.CategoriesReference is not null && group.CategoriesReference is not null &&
            group.CategoriesReference.Any() && group.SubCategories.Any())
            group.SetLinkedGroups(await GetLinkedGroups(group));

        return HttpResponse.Successful(group);
    }

    public async Task<List<Group>> GetLinkedGroups(Group group)
    {
        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
        ContentfulCollection<ContentfulGroup> entries = await GetAllEntriesAsync(_client, builder);
        IEnumerable<ContentfulGroup> contentfulGroups = entries as IEnumerable<ContentfulGroup> ?? entries.ToList();

        contentfulGroups = contentfulGroups.Where(groupItem =>_dateComparer.DateNowIsNotBetweenHiddenRange(groupItem.DateHiddenFrom, groupItem.DateHiddenTo));

        IEnumerable<Group> groupList = contentfulGroups.Select(group => _groupFactory.ToModel(group));

        IEnumerable<Group> linkedGroups = groupList.Where(group =>
            group.CategoriesReference.Any(category =>
                string.IsNullOrEmpty(group.CategoriesReference[0].Slug) ||
                category.Slug.ToLower().Equals(group.CategoriesReference[0].Slug.ToLower()))
            && group.SubCategories.Any(category =>
                string.IsNullOrEmpty(group.SubCategories[0].Slug) ||
                category.Slug.ToLower().Equals(group.SubCategories[0].Slug.ToLower()))
            && !group.Slug.Equals(group.Slug));

        return linkedGroups.ToList();
    }

    //TODO:: look at the Tags lowercase potential issue
    public async Task<HttpResponse> GetGroupResults(GroupSearch groupSearch, string slugs)
    {
        GroupResults groupResults = new();

        QueryBuilder<ContentfulGroup> builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);

        if (groupSearch.Longitude.Equals(0) && groupSearch.Latitude.Equals(0))
            builder = builder.FieldEquals("fields.mapPosition[near]",
                $"{groupSearch.Latitude},{groupSearch.Longitude}{(groupSearch.Location.ToLower().Equals(Groups.Location) ? ",10" : ",3.2")}");

        string[] subCategoriesArray = groupSearch.SubCategories.Split(',');
        
        IEnumerable<string> subCategoriesList =
            subCategoriesArray.Where(subCategory => !string.IsNullOrWhiteSpace(subCategory));

        if (!string.IsNullOrEmpty(slugs))
        {
            string[] slugsList = slugs.Split(',');
            builder = builder.FieldIncludes("fields.slug", slugsList);
        }

        ContentfulCollection<ContentfulGroup> entries = await GetAllEntriesAsync(_client, builder);

        if (entries is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No groups found");

        List<Group> groupsWithNoCoordinates = new();
        ContentfulCollection<ContentfulGroup> noCoordinatesEntries = entries;

        if (groupSearch.Location.ToLower().Equals(Groups.Location))
        {
            QueryBuilder<ContentfulGroup> noCoordinatesBuilder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);

            if (!string.IsNullOrEmpty(slugs))
                noCoordinatesEntries = entries;
            else
                noCoordinatesEntries = await GetAllEntriesAsync(_client, noCoordinatesBuilder);

            groupsWithNoCoordinates = noCoordinatesEntries.Select(group => _groupFactory.ToModel(group))
                .Where(_ => _.MapPosition.Lat.Equals(0) && _.MapPosition.Lon.Equals(0))
                .Where(group => group.CategoriesReference.Any(category =>
                    string.IsNullOrEmpty(groupSearch.Category) ||
                    category.Slug.ToLower().Equals(groupSearch.Category.ToLower())))
                .Where(group =>
                    string.IsNullOrWhiteSpace(groupSearch.Tags) || group.Tags.Contains(groupSearch.Tags.ToLower()))
                .Where(group => _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo))
                .Where(group =>
                    string.IsNullOrEmpty(groupSearch.GetInvolved) ||
                    group.Volunteering && groupSearch.GetInvolved.Equals("yes"))
                .Where(group => string.IsNullOrEmpty(groupSearch.Organisation) ||
                                group.Organisation is not null &&
                                group.Organisation.Slug.Equals(groupSearch.Organisation))
                .Where(group => !subCategoriesList.Any() ||
                                group.SubCategories.Any(category => subCategoriesList.Contains(category.Slug)))
                .ToList();

            groupsWithNoCoordinates = groupsWithNoCoordinates.OrderBy(group => group.Name).Distinct().ToList();
        }

        List<Group> groups =
            entries.Select(group => _groupFactory.ToModel(group))
                .Where(group => group.CategoriesReference.Any(category =>
                    string.IsNullOrEmpty(groupSearch.Category) ||
                    category.Slug.ToLower().Equals(groupSearch.Category.ToLower())))
                .Where(group =>
                    string.IsNullOrWhiteSpace(groupSearch.Tags) || group.Tags.Contains(groupSearch.Tags.ToLower()))
                .Where(group => _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo))
                .Where(group =>
                    string.IsNullOrEmpty(groupSearch.GetInvolved) ||
                    group.Volunteering && groupSearch.GetInvolved.Equals("yes"))
                .Where(group => string.IsNullOrEmpty(groupSearch.Organisation) || group.Organisation is not null &&
                    group.Organisation.Slug.Equals(groupSearch.Organisation))
                .Where(group =>
                    !subCategoriesList.Any() ||
                    group.SubCategories.Any(category => subCategoriesList.Contains(category.Slug)))
                .Where(_ => !_.MapPosition.Lat.Equals(0) && !_.MapPosition.Lon.Equals(0))
                .ToList();

        if (groupsWithNoCoordinates.Count > 0)
            groups.AddRange(groupsWithNoCoordinates);

        switch (!string.IsNullOrEmpty(groupSearch.Order) ? groupSearch.Order.ToLower() : "name a-z")
        {
            case "name a-z":
                groups = groups.OrderBy(group => group.Name).ToList();
                break;
            case "name z-a":
                groups = groups.OrderByDescending(group => group.Name).ToList();
                break;
            case "nearest":
                break;
            default:
                groups = groups.OrderBy(group => group.Name).ToList();
                break;
        }

        groupResults.Groups = groups;
        groupResults.AvailableSubCategories = groups.SelectMany(group => group.SubCategories ?? new List<GroupSubCategory>()).ToList();

        List<GroupCategory> groupCategoryResults = await GetGroupCategories();

        if (!string.IsNullOrEmpty(groupSearch.Category) && groupCategoryResults.All(group => !string.Equals(group.Slug, groupSearch.Category, StringComparison.CurrentCultureIgnoreCase)))
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories found");

        groupResults.Categories = groupCategoryResults;

        return HttpResponse.Successful(groupResults);
    }

    public async Task<HttpResponse> GetAdministratorsGroups(string email)
    {
        QueryBuilder<ContentfulGroup> builder =
            new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldExists("fields.groupAdministrators").Include(1).Limit(ContentfulQueryValues.LIMIT_MAX);

        ContentfulCollection<ContentfulGroup> contentfulGroups = await _client.GetEntries(builder);

        List<ContentfulGroup> groups = contentfulGroups.Where(group => group.GroupAdministrators?.Items is not null && group.GroupAdministrators.Items
                                        .Any(item => item is not null && item.Email.ToUpper().Equals(email.ToUpper()))).ToList();

        IEnumerable<Group> result = groups.Select(group => _groupFactory.ToModel(group));

        return HttpResponse.Successful(result.ToList());
    }

    public async Task<List<GroupCategory>> GetGroupCategories() =>
        await _cache.GetFromCacheOrDirectlyAsync("group-categories", GetGroupCategoriesDirect, _groupsTimeout);

    public async Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategories() =>
        await _cache.GetFromCacheOrDirectlyAsync("contentful-group-categories", GetContentfulGroupCategoriesDirect, _groupsTimeout);

    public async Task<List<Group>> GetLinkedGroupsByOrganisation(string slug)
    {
        HttpResponse response = await Get();
        List<Group> groups = response.Get<List<Group>>();

        groups = groups.Where(group => group.Organisation.Slug.Equals(slug))
                    .OrderBy(group => group.Name)
                    .ToList();

        return groups;
    }

    private async Task<List<GroupCategory>> GetGroupCategoriesDirect()
    {
        QueryBuilder<ContentfulGroupCategory> groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
        ContentfulCollection<ContentfulGroupCategory> groupCategoryEntries = await _client.GetEntries(groupCategoryBuilder);

        List<GroupCategory> groupCategoryList = groupCategoryEntries.Select(gc => _groupCategoryFactory.ToModel(gc))
                                                    .OrderBy(category => category.Name).ToList();

        return !groupCategoryList.Any()
            ? null
            : groupCategoryList;
    }

    private async Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategoriesDirect()
    {
        QueryBuilder<ContentfulGroupCategory> groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
        ContentfulCollection<ContentfulGroupCategory> result = await _client.GetEntries(groupCategoryBuilder);

        return !result.Any()
            ? null 
            : result;
    }
}