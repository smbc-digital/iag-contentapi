namespace StockportContentApi.Models;

[ExcludeFromCodeCoverage]
public class Healthcheck(string appVersion, string sha, string environment, List<RedisValueData> redisKeys)
{
    public readonly string AppVersion = appVersion;
    public readonly string SHA = sha;
    public readonly string Environment = environment;
    public readonly List<RedisValueData> RedisValueData = redisKeys;
}