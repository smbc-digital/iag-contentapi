namespace StockportContentApi.Config;

[ExcludeFromCodeCoverage]
public class RedirectBusinessIds(List<string> businessIds)
{
    public List<string> BusinessIds { get; } = businessIds;
}
