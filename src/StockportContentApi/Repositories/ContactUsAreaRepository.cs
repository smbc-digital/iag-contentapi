namespace StockportContentApi.Repositories;

public interface IContactUsAreaRepository
{
    Task<HttpResponse> GetContactUsArea();
}

public class ContactUsAreaRepository(ContentfulConfig config,
                                    IContentfulClientManager clientManager,
                                    IContentfulFactory<ContentfulContactUsArea, ContactUsArea> contentfulFactory) : IContactUsAreaRepository
{
    private readonly IContentfulClient _client = clientManager.GetClient(config);
    private readonly IContentfulFactory<ContentfulContactUsArea, ContactUsArea> _contentfulFactory = contentfulFactory;

    public async Task<HttpResponse> GetContactUsArea()
    {
        QueryBuilder<ContentfulContactUsArea> builder = new QueryBuilder<ContentfulContactUsArea>().ContentTypeIs("contactUsArea").Include(3);
        ContentfulCollection<ContentfulContactUsArea> entries = await _client.GetEntries(builder);
        ContentfulContactUsArea entry = entries.FirstOrDefault();
        ContactUsArea contactUsArea = _contentfulFactory.ToModel(entry);

        if (contactUsArea is null)
            return HttpResponse.Failure(HttpStatusCode.NotFound, "No contact us area found");

        return HttpResponse.Successful(contactUsArea);
    }
}