namespace StockportContentApi.Repositories;

public class LandingPageRepository : BaseRepository
{
    private readonly IContentfulFactory<ContentfulLandingPage, LandingPage> _contentfulFactory;
    private readonly IContentfulClient _client;
    private readonly IContentfulFactory<ContentfulReference, SubItem> _subItemFactory;

    public LandingPageRepository(ContentfulConfig config,
        IContentfulFactory<ContentfulLandingPage, LandingPage> contentfulFactory,
        IContentfulClientManager contentfulClientManager,
        IContentfulFactory<ContentfulReference, SubItem> subItemFactory)
    {
        _contentfulFactory = contentfulFactory;
        _client = contentfulClientManager.GetClient(config);
        _subItemFactory = subItemFactory;
    }

    public async Task<HttpResponse> GetLandingPage(string slug)
    {
        QueryBuilder<ContentfulLandingPage> builder = new QueryBuilder<ContentfulLandingPage>().ContentTypeIs("landingPage").FieldEquals("fields.slug", slug).Include(3);
        ContentfulCollection<ContentfulLandingPage> entries = await _client.GetEntries(builder);
        ContentfulLandingPage entry = entries.FirstOrDefault();
        
        if(entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No Landing Page found");
        
        if(entry.Content?.Any() is true)
        {
            string jsonString = entry.Content["content"].ToString();

            List<ContentItem> contentItems = JsonConvert.DeserializeObject<List<ContentItem>>(jsonString);

            if (contentItems?.Any() is true)
            {
                List<SubItem> contentBlocks = new();

                foreach (ContentItem contentItem in contentItems)
                {
                    if (contentItem is not null && contentItem.Data is not null && contentItem.Data.Target is not null)
                        contentBlocks.Add(await GetContentBlock(contentItem.Data.Target.Slug));
                }

                entry.ContentBlocks = contentBlocks;
            }
        }

        LandingPage landingPage = _contentfulFactory.ToModel(entry);

        return landingPage is null
            ? HttpResponse.Failure(HttpStatusCode.NotFound, $"Landing page not found {slug}")
            : HttpResponse.Successful(landingPage);  
    }

    internal async Task<SubItem> GetContentBlock(string slug)
    {
        QueryBuilder<ContentfulReference> builder = new QueryBuilder<ContentfulReference>().ContentTypeIs("contentBlock").FieldEquals("fields.slug", slug).Include(1);
        ContentfulCollection<ContentfulReference> entries = await GetAllEntriesAsync(_client, builder);

        IEnumerable<ContentfulReference> contentfulEntries = entries as IEnumerable<ContentfulReference> ?? entries.ToList();
        return contentfulEntries.Select(_ => _subItemFactory.ToModel(_)).ToList().FirstOrDefault();
    }
}