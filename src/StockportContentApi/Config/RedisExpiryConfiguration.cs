namespace StockportContentApi.Config;

[ExcludeFromCodeCoverage]
public class RedisExpiryConfiguration
{
    public int News { get; set; }
    public int Events { get; set; }
    public int Groups { get; set; }
    public int Articles { get; set; }
    public int AtoZ { get; set; }
    public int Directory { get; set; }
}