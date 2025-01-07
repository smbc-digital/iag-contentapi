namespace StockportContentApi.Repositories;

public interface IManagementRepository
{
    Task<HttpResponse> CreateOrUpdate(dynamic content, SystemProperties systemProperties);
    Task<HttpResponse> Delete(SystemProperties systemProperties);
    Task<int> GetVersion(string entryId);
}

[ExcludeFromCodeCoverage]
public class ManagementRepository(ContentfulConfig config,
                                IContentfulClientManager client,
                                ILogger logger) : IManagementRepository
{
    private readonly IContentfulManagementClient _client = client.GetManagementClient(config);
    private readonly ILogger _logger = logger;

    public async Task<HttpResponse> CreateOrUpdate(dynamic content, SystemProperties systemProperties)
    {
        Entry<dynamic> entry = new()
        {
            Fields = content,
            SystemProperties = systemProperties
        };

        try
        {
            Entry<dynamic> group = await _client.CreateOrUpdateEntry(entry, null, null, systemProperties.Version);

            if (group.SystemProperties.Version is not null)
                await _client.PublishEntry(entry.SystemProperties.Id, group.SystemProperties.Version.Value);

            return HttpResponse.Successful(group);
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(0), ex, "An unexpected error occurred while performing the get operation");

            return HttpResponse.Failure(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<HttpResponse> Delete(SystemProperties systemProperties)
    {
        try
        {
            await _client.UnpublishEntry(systemProperties.Id, systemProperties.Version.Value);
            await _client.DeleteEntry(systemProperties.Id, systemProperties.Version.Value);
            
            return HttpResponse.Successful($"Successfully Deleted Entry: {systemProperties.Id}");
        }
        catch (Exception ex)
        {
            _logger.LogError(new EventId(0), ex, "An unexpected error occured while performing the get operation");

            return HttpResponse.Failure(HttpStatusCode.InternalServerError, ex.Message);
        }
    }

    public async Task<int> GetVersion(string entryId)
    {
        Entry<dynamic> managementGroup = await _client.GetEntry(entryId);

        return managementGroup.SystemProperties.Version ?? 0;
    }
}