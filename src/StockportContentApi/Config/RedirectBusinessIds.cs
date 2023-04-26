namespace StockportContentApi.Config;

public class RedirectBusinessIds
{
    public List<string> BusinessIds { get; }

    public RedirectBusinessIds(List<string> businessIds)
    {
        BusinessIds = businessIds;
    }
}
