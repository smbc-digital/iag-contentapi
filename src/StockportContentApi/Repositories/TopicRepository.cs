namespace StockportContentApi.Repositories;

public class TopicRepository
{
    private readonly IContentfulFactory<ContentfulTopic, Topic> _topicFactory;
    private readonly IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap> _topicSiteMapFactory;
    private readonly IContentfulClient _client;

    public TopicRepository(
        ContentfulConfig config, IContentfulClientManager clientManager,
        IContentfulFactory<ContentfulTopic, Topic> topicFactory,
        IContentfulFactory<ContentfulTopicForSiteMap, TopicSiteMap> topicSiteMapFactory)
    {
        _client = clientManager.GetClient(config);
        _topicFactory = topicFactory;
        _topicSiteMapFactory = topicSiteMapFactory;
    }

    public async Task<HttpResponse> GetTopicByTopicSlug(string slug)
    {
        QueryBuilder<ContentfulTopic> builder = new QueryBuilder<ContentfulTopic>().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(2);
        ContentfulCollection<ContentfulTopic> entries = await _client.GetEntries(builder);

        ContentfulTopic entry = entries.FirstOrDefault();

        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No topic found for '{slug}'");

        Topic model = _topicFactory.ToModel(entry);

        return HttpResponse.Successful(model);
    }

    public async Task<HttpResponse> Get()
    {
        QueryBuilder<ContentfulTopicForSiteMap> builder = new QueryBuilder<ContentfulTopicForSiteMap>().ContentTypeIs("topic").Include(2);
        ContentfulCollection<ContentfulTopicForSiteMap> entries = await _client.GetEntries(builder);
        IEnumerable<ContentfulTopicForSiteMap> contentfulTopics = entries as IEnumerable<ContentfulTopicForSiteMap> ?? entries.ToList();
        IEnumerable<TopicSiteMap> topics = GetAllTopics(contentfulTopics.ToList());

        return entries is null || !contentfulTopics.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Topics found")
            : HttpResponse.Successful(topics);
    }

    private IEnumerable<TopicSiteMap> GetAllTopics(List<ContentfulTopicForSiteMap> entries) => 
        entries.Select(entry => _topicSiteMapFactory.ToModel(entry)).ToList();
}