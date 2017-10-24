namespace StockportContentApi.Config
{
    public interface ISimpleContentfulConfig
    {
        
    }

    public class SimpleContentfulConfig : ISimpleContentfulConfig
    {
        public string SpaceKey { get; }
        public string AccessKey { get; }
        public string ManagementKey { get; }

        public SimpleContentfulConfig(string spaceKey, string accessKey, string managementKey)
        {
            SpaceKey = spaceKey;
            AccessKey = accessKey;
            ManagementKey = managementKey;
        }
    }
}
