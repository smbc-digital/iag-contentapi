namespace StockportContentApi.Model;

public class Healthcheck
{
    public readonly string AppVersion;
    public readonly string SHA;
    public readonly string Environment;
    public readonly List<RedisValueData> RedisValueData;

    public Healthcheck(string appVersion, string sha, string environment, List<RedisValueData> redisKeys)
    {
        AppVersion = appVersion;
        SHA = sha;
        Environment = environment;
        RedisValueData = redisKeys;
    }
}