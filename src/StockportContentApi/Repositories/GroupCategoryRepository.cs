namespace StockportContentApi.Repositories;

public interface IGroupCategoryRepository
{
    Task<HttpResponse> GetGroupCategories();
}

public class GroupCategoryRepository(ContentfulConfig config,
                                    IContentfulFactory<ContentfulGroupCategory, GroupCategory> contentfulFactory,
                                    IContentfulClientManager contentfulClientManager) : IGroupCategoryRepository
{
    private readonly IContentfulFactory<ContentfulGroupCategory, GroupCategory> _contentfulFactory = contentfulFactory;
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);

    public async Task<HttpResponse> GetGroupCategories()
    {
        QueryBuilder<ContentfulGroupCategory> builder = new QueryBuilder<ContentfulGroupCategory>().ContentTypeIs("groupCategory");
        ContentfulCollection<ContentfulGroupCategory> entries = await _client.GetEntries(builder);

        if (entries is null || !entries.Any())
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No group catogories found");

        List<GroupCategory> groupCategories = entries.Select(groupCatogory => _contentfulFactory.ToModel(groupCatogory)).ToList();
        groupCategories = groupCategories.OrderBy(group => group.Name).ToList();

        return HttpResponse.Successful(groupCategories);
    }
}
