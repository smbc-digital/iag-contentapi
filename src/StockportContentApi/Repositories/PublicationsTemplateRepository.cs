namespace StockportContentApi.Repositories;

public interface IPublicationsTemplateRepository
{
    Task<HttpResponse> GetPublicationsTemplate(string slug, string tagId);
}

public class PublicationsTemplateRepository(
        ContentfulConfig config,
        IContentfulFactory<ContentfulPublicationsTemplate, PublicationsTemplate> contentfulFactory,
        IContentfulClientManager contentfulClientManager) : BaseRepository, IPublicationsTemplateRepository
{
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulPublicationsTemplate, PublicationsTemplate> _contentfulFactory = contentfulFactory;

    public async Task<HttpResponse> GetPublicationsTemplate(string slug, string tagId)
    {
        QueryBuilder<ContentfulPublicationsTemplate> builder = new QueryBuilder<ContentfulPublicationsTemplate>()
            .ContentTypeIs("publicationsTemplate")
            .FieldEquals("fields.slug", slug)
            .Include(10);
        
        ContentfulCollection<ContentfulPublicationsTemplate> entries = await _client.GetEntries(builder);
        ContentfulPublicationsTemplate entry = entries.FirstOrDefault();
        
        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No publications template found");
        
        PublicationsTemplate publicationsTemplate = _contentfulFactory.ToModel(entry);

        if (publicationsTemplate is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"Publications template not found {slug}");
        
        return HttpResponse.Successful(publicationsTemplate);
    }
}