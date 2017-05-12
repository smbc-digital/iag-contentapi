namespace StockportContentApi.Model
{
    public class Healthcheck
    {
        public readonly string AppVersion;
        public readonly string SHA;
        public readonly string Environment;

        public Healthcheck(string appVersion, string sha, string environment)
        {
            AppVersion = appVersion;
            SHA = sha;
            Environment = environment;
        }
    }
}