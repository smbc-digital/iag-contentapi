namespace StockportContentApi.Repositories;

public abstract class BaseRepository
{
    public async Task<ContentfulCollection<T>> GetAllEntriesAsync<T>(Contentful.Core.IContentfulClient _client, QueryBuilder<T> builder, Microsoft.Extensions.Logging.ILogger logger = null)
    {
        if (!BuilderHasProperty(builder, "limit"))
        {
            builder.Limit(ContentfulQueryValues.LIMIT_MAX);
        }

        builder.Skip(0);
        ContentfulCollection<T> result;
        try
        {
            result = await _client.GetEntries(builder);
        }
        catch (Exception ex)
        {
            logger?.LogError(ex, $"Could not get Entries: {ex.Message}");
            throw;
        }

        return result;
    }

    private bool BuilderHasProperty<T>(QueryBuilder<T> builder, string property)
    {
        return builder.Build().Contains(property);
    }
}
