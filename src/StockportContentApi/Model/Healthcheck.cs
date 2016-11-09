namespace StockportContentApi.Model
{
    public class Healthcheck
    {
        public readonly string AppVersion;
        public readonly string SHA;

        public Healthcheck(string appVersion, string sha)
        {
            AppVersion = appVersion;
            SHA = sha;
        }
    }
}