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
        var builder = new QueryBuilder<ContentfulTopic>().ContentTypeIs("topic").FieldEquals("fields.slug", slug).Include(2);
        var entries = await _client.GetEntries(builder);

        var entry = entries.FirstOrDefault();

        if (entry == null) 
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"No topic found for '{slug}'");

        var model = _topicFactory.ToModel(entry);

        return HttpResponse.Successful(model);
    }

    public async Task<HttpResponse> Get()
    {
        var builder = new QueryBuilder<ContentfulTopicForSiteMap>().ContentTypeIs("topic").Include(2);
        var entries = await _client.GetEntries(builder);
        var contentfulTopics = entries as IEnumerable<ContentfulTopicForSiteMap> ?? entries.ToList();

        var topics = GetAllTopics(contentfulTopics.ToList());
        return entries == null || !contentfulTopics.Any()
            ? HttpResponse.Failure(HttpStatusCode.NotFound, "No Topics found")
            : HttpResponse.Successful(topics);
    }

    private IEnumerable<TopicSiteMap> GetAllTopics(List<ContentfulTopicForSiteMap> entries)
    {
        var entriesList = new List<TopicSiteMap>();
        foreach (var entry in entries)
        {
            var topicItem = _topicSiteMapFactory.ToModel(entry);
            entriesList.Add(topicItem);
        }

        return entriesList;
    }
}