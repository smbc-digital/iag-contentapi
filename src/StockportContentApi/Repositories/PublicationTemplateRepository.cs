namespace StockportContentApi.Repositories;

public interface IPublicationTemplateRepository
{
    Task<HttpResponse> GetPublicationTemplate(string slug, string tagId);
}

public class PublicationTemplateRepository(
        ContentfulConfig config,
        IContentfulFactory<ContentfulPublicationTemplate, PublicationTemplate> contentfulFactory,
        IContentfulClientManager contentfulClientManager) : BaseRepository, IPublicationTemplateRepository
{
    private readonly IContentfulClient _client = contentfulClientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulPublicationTemplate, PublicationTemplate> _contentfulFactory = contentfulFactory;

    public async Task<HttpResponse> GetPublicationTemplate(string slug, string tagId)
    {
        QueryBuilder<ContentfulPublicationTemplate> builder = new QueryBuilder<ContentfulPublicationTemplate>()
            .ContentTypeIs("publicationTemplate")
            .FieldEquals("fields.slug", slug)
            .Include(10);
        
        ContentfulCollection<ContentfulPublicationTemplate> entries = await _client.GetEntries(builder);
        ContentfulPublicationTemplate entry = entries.FirstOrDefault();
        
        if (entry is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No publication template found");

        PublicationTemplate publicationTemplate = _contentfulFactory.ToModel(entry);

        if (publicationTemplate is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, $"Publication template not found {slug}");
        
        return HttpResponse.Successful(publicationTemplate);
    }
}