﻿namespace StockportContentApi.Repositories;

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
    private readonly Contentful.Core.IContentfulClient _client;
    private readonly DateComparer _dateComparer;
    private readonly IContentfulFactory<ContentfulGroup, Group> _groupFactory;
    private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _groupCategoryFactory;
    private readonly IContentfulFactory<ContentfulGroupHomepage, GroupHomepage> _groupHomepageContentfulFactory;
    private readonly EventRepository _eventRepository;
    private readonly ICache _cache;
    private IConfiguration _configuration;
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
        _dateComparer = new DateComparer(timeProvider);
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
        var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulGroups = entries as IEnumerable<ContentfulGroup> ?? entries.ToList();

        contentfulGroups =
            contentfulGroups.Where(
                group => _dateComparer.DateNowIsNotBetweenHiddenRange(group.DateHiddenFrom, group.DateHiddenTo));

        var groupList = contentfulGroups.Select(g => _groupFactory.ToModel(g));

        return entries == null || !groupList.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Groups found")
            : HttpResponse.Successful(groupList.ToList());
    }

    public async Task<ContentfulGroup> GetContentfulGroup(string slug)
    {
        var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);
        var entries = await _client.GetEntries(builder);
        var entry = entries.FirstOrDefault();

        return entry;
    }

    public async Task<HttpResponse> GetGroupHomepage()
    {
        var builder = new QueryBuilder<ContentfulGroupHomepage>().ContentTypeIs("groupHomepage").Include(1);
        var entries = await _client.GetEntries(builder);
        var entry = entries.ToList().First();

        return entry == null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No event homepage found")
            : HttpResponse.Successful(_groupHomepageContentfulFactory.ToModel(entry));
    }

    public async Task<HttpResponse> GetGroup(string slug, bool onlyActive)
    {
        var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldEquals("fields.slug", slug).Include(1);

        var entries = await _client.GetEntries(builder);

        var entry = onlyActive
            ? entries.FirstOrDefault(g => _dateComparer.DateNowIsNotBetweenHiddenRange(g.DateHiddenFrom, g.DateHiddenTo))
            : entries.FirstOrDefault();

        if (entry == null) return HttpResponse.Failure(HttpStatusCode.NotFound, $"No group found for '{slug}'");

        var group = _groupFactory.ToModel(entry);
        group.SetEvents(await _eventRepository.GetLinkedEvents<Group>(slug));

        var twitterUser = group.Twitter;
        var faceBookUser = group.Facebook;
        if (twitterUser != null && twitterUser.StartsWith("@"))
        {
            twitterUser = twitterUser.Replace("@", "/");
            group.Twitter = @"https://www.twitter.com" + twitterUser;
        }

        if (!string.IsNullOrEmpty(faceBookUser) && faceBookUser.StartsWith("/"))
        {
            faceBookUser = faceBookUser.Replace("/", "");
            group.Facebook = @"https://www.facebook.co.uk" + faceBookUser;
        }

        if (!string.IsNullOrEmpty(faceBookUser) && (!faceBookUser.StartsWith("http") || !faceBookUser.StartsWith("http")))
        {
            group.Facebook = @"https://" + faceBookUser;
        }

        if (group.CategoriesReference != null && group.CategoriesReference != null && group.CategoriesReference.Any() && group.SubCategories.Any())
        {
            group.SetLinkedGroups(await GetLinkedGroups(group));
        }

        return HttpResponse.Successful(group);
    }

    public async Task<List<Group>> GetLinkedGroups(Group group)
    {
        var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
        var entries = await GetAllEntriesAsync(_client, builder);
        var contentfulGroups = entries as IEnumerable<ContentfulGroup> ?? entries.ToList();

        contentfulGroups =
            contentfulGroups.Where(
                groupItem => _dateComparer.DateNowIsNotBetweenHiddenRange(groupItem.DateHiddenFrom, groupItem.DateHiddenTo));

        var groupList = contentfulGroups.Select(g => _groupFactory.ToModel(g));

        var linkeddGroups = groupList.Where(g => g.CategoriesReference.Any(c => string.IsNullOrEmpty(group.CategoriesReference[0].Slug) || c.Slug.ToLower() == group.CategoriesReference[0].Slug.ToLower())
                                   && g.SubCategories.Any(c => string.IsNullOrEmpty(group.SubCategories[0].Slug) || c.Slug.ToLower() == group.SubCategories[0].Slug.ToLower())
                                   && g.Slug != group.Slug);

        return linkeddGroups.ToList();
    }

    //TODO:: look at the Tags lowercase potential issue
    public async Task<HttpResponse> GetGroupResults(GroupSearch groupSearch, string slugs)
    {
        var groupResults = new GroupResults();

        var builder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);

        if (groupSearch.Longitude != 0 && groupSearch.Latitude != 0)
        {
            builder = builder.FieldEquals("fields.mapPosition[near]",
                groupSearch.Latitude +
                "," +
                groupSearch.Longitude +
                (groupSearch.Location.ToLower() == Constants.Groups.Location ? ",10" : ",3.2"));
        }

        var subCategoriesArray = groupSearch.SubCategories.Split(',');
        var subCategoriesList = subCategoriesArray.Where(c => !string.IsNullOrWhiteSpace(c));

        if (!string.IsNullOrEmpty(slugs))
        {
            var slugsList = slugs.Split(',');
            builder = builder.FieldIncludes("fields.slug", slugsList);
        }

        var entries = await GetAllEntriesAsync(_client, builder);

        if (entries == null) return HttpResponse.Failure(HttpStatusCode.NotFound, "No groups found");

        var groupsWithNoCoordinates = new List<Group>();
        var noCoordinatesEntries = entries;
        if (groupSearch.Location.ToLower() == Constants.Groups.Location)
        {
            var noCoordinatesBuilder = new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").Include(1);
            if (!string.IsNullOrEmpty(slugs))
            {
                noCoordinatesEntries = entries;
            }
            else
            {
                noCoordinatesEntries = await GetAllEntriesAsync(_client, noCoordinatesBuilder);
            }

            groupsWithNoCoordinates = noCoordinatesEntries.Select(g => _groupFactory.ToModel(g))
                .Where(_ => _.MapPosition.Lat == 0 && _.MapPosition.Lon == 0)
                .Where(g => g.CategoriesReference.Any(c =>
                    string.IsNullOrEmpty(groupSearch.Category) || c.Slug.ToLower() == groupSearch.Category.ToLower()))
                .Where(g => string.IsNullOrWhiteSpace(groupSearch.Tags) || g.Tags.Contains(groupSearch.Tags.ToLower()))
                .Where(g => _dateComparer.DateNowIsNotBetweenHiddenRange(g.DateHiddenFrom, g.DateHiddenTo))
                .Where(g => groupSearch.GetInvolved == string.Empty || (g.Volunteering && groupSearch.GetInvolved == "yes"))
                .Where(g => groupSearch.Organisation == string.Empty ||
                            (g.Organisation != null && g.Organisation.Slug == groupSearch.Organisation))
                .Where(g => !subCategoriesList.Any() ||
                            g.SubCategories.Any(c => subCategoriesList.Contains(c.Slug)))
                .ToList();

            groupsWithNoCoordinates = groupsWithNoCoordinates.OrderBy(g => g.Name).Distinct().ToList();
        }

        var groups =
             entries.Select(g => _groupFactory.ToModel(g))
                    .Where(g => g.CategoriesReference.Any(c => string.IsNullOrEmpty(groupSearch.Category) || c.Slug.ToLower() == groupSearch.Category.ToLower()))
                    .Where(g => string.IsNullOrWhiteSpace(groupSearch.Tags) || g.Tags.Contains(groupSearch.Tags.ToLower()))
                    .Where(g => _dateComparer.DateNowIsNotBetweenHiddenRange(g.DateHiddenFrom, g.DateHiddenTo))
                    .Where(g => groupSearch.GetInvolved == string.Empty || (g.Volunteering && groupSearch.GetInvolved == "yes"))
                    .Where(g => groupSearch.Organisation == string.Empty || (g.Organisation != null && g.Organisation.Slug == groupSearch.Organisation))
                    .Where(g => !subCategoriesList.Any() || g.SubCategories.Any(c => subCategoriesList.Contains(c.Slug)))
                 .Where(_ => _.MapPosition.Lat != 0 && _.MapPosition.Lon != 0)
                    .ToList();

        if (groupsWithNoCoordinates.Count > 0) groups.AddRange(groupsWithNoCoordinates);
        switch (!string.IsNullOrEmpty(groupSearch.Order) ? groupSearch.Order.ToLower() : "name a-z")
        {
            case "name a-z":
                groups = groups.OrderBy(g => g.Name).ToList();
                break;
            case "name z-a":
                groups = groups.OrderByDescending(g => g.Name).ToList();
                break;
            case "nearest":
                break;
            default:
                groups = groups.OrderBy(g => g.Name).ToList();
                break;
        }
        //if(!string.IsNullOrEmpty(category))
        groupResults.Groups = groups;

        groupResults.AvailableSubCategories = groups.SelectMany(g => g.SubCategories ?? new List<GroupSubCategory>()).ToList();

        var groupCategoryResults = await GetGroupCategories();

        if (!string.IsNullOrEmpty(groupSearch.Category) && groupCategoryResults.All(g => !string.Equals(g.Slug, groupSearch.Category, StringComparison.CurrentCultureIgnoreCase)))
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No categories found");

        groupResults.Categories = groupCategoryResults;

        return HttpResponse.Successful(groupResults);
    }

    public async Task<HttpResponse> GetAdministratorsGroups(string email)
    {
        var builder =
            new QueryBuilder<ContentfulGroup>().ContentTypeIs("group").FieldExists("fields.groupAdministrators")
                .Include(1)
                .Limit(ContentfulQueryValues.LIMIT_MAX);

        var contentfulGroups = await _client.GetEntries(builder);

        var groups = contentfulGroups.Where(g => g.GroupAdministrators?.Items != null && g.GroupAdministrators.Items.Any(i => i != null && i.Email.ToUpper() == email.ToUpper())).ToList();

        var result = groups.Select(g => _groupFactory.ToModel(g));

        return HttpResponse.Successful(result.ToList());
    }

    public async Task<List<GroupCategory>> GetGroupCategories()
    {
        return await _cache.GetFromCacheOrDirectlyAsync("group-categories", GetGroupCategoriesDirect, _groupsTimeout);
    }

    public async Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategories()
    {
        return await _cache.GetFromCacheOrDirectlyAsync("contentful-group-categories", GetContentfulGroupCategoriesDirect, _groupsTimeout);
    }

    public async Task<List<Group>> GetLinkedGroupsByOrganisation(string slug)
    {
        var response = await Get();
        var groups = response.Get<List<Group>>();

        groups = groups.Where(g => g.Organisation.Slug == slug)
            .OrderBy(g => g.Name)
            .ToList();

        return groups;
    }

    private async Task<List<GroupCategory>> GetGroupCategoriesDirect()
    {
        var groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
        var groupCategoryEntries = await _client.GetEntries(groupCategoryBuilder);

        var groupCategoryList = groupCategoryEntries.Select(gc => _groupCategoryFactory.ToModel(gc))
            .OrderBy(c => c.Name).ToList();

        return !groupCategoryList.Any() ? null : groupCategoryList;
    }

    private async Task<ContentfulCollection<ContentfulGroupCategory>> GetContentfulGroupCategoriesDirect()
    {
        var groupCategoryBuilder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory").Include(1);
        var result = await _client.GetEntries(groupCategoryBuilder);

        return !result.Any() ? null : result;
    }
}
