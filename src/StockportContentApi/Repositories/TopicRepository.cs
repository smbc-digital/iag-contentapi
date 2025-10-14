namespace StockportContentApi.Repositories;

public interface ITopicRepository
{
    Task<HttpResponse> GetTopicByTopicSlug(string slug, string tagId);
    Task<HttpResponse> Get(string tagId);
}

public class TopicRepository(ContentfulConfig config, IContentfulClientManager clientManager,
                            IContentfulFactory<ContentfulTopic, Topic> topicFactory,
                            IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap> topicSiteMapFactory) : ITopicRepository
{
    private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory = topicFactory;
    private readonly IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap> _topicSiteMapFactory = topicSiteMapFactory;
    private readonly IContentfulClient _client = clientManager.GetClient(config);

    public async Task<HttpResponse> GetTopicByTopicSlug(string slug, string tagId)
    {
        QueryBuilder<ContentfulTopic> builder = new QueryBuilder<ContentfulTopic>()
            .ContentTypeIs("topic")
            .FieldEquals("fields.slug", slug)
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(2);
        
        ContentfulCollection<ContentfulTopic> entries = await _client.GetEntries(builder);

        ContentfulTopic entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No topic found for '{slug}'");

        Topic model = _topicFactory.ToModel(entry);

        return HttpResponse.Successful(model);
    }

    public async Task<HttpResponse> Get(string tagId)
    {
        QueryBuilder<ContentfulTopicForSiteMap> builder = new QueryBuilder<ContentfulTopicForSiteMap>()
            .ContentTypeIs("topic")
            .FieldExists("metadata.tags")
            .FieldEquals("metadata.tags.sys.id[in]", tagId)
            .Include(2);
        
        ContentfulCollection<ContentfulTopicForSiteMap> entries = await _client.GetEntries(builder);
        IEnumerable<ContentfulTopicForSiteMap> contentfulTopics = entries as IEnumerable<ContentfulTopicForSiteMap> ?? entries.ToList();
        IEnumerable<TopicSiteMap> topics = GetAllTopics(contentfulTopics.ToList());

        return entries is null || !contentfulTopics.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Topics found")
            : HttpResponse.Successful(topics);
    }

    private IEnumerable<TopicSiteMap> GetAllTopics(List<ContentfulTopicForSiteMap> entries) => 
        entries.Select(_topicSiteMapFactory.ToModel).ToList();
}