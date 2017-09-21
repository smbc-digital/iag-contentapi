using StockportContentApi.Attributes;

namespace StockportContentApi.Model
{
    public class GroupAdministratorItems
    {
        [SensitiveData]
        public string Email { get; set; } = string.Empty;
        [SensitiveData]
        public string Permission { get; set; } = string.Empty;
        [SensitiveData]
        public string Name { get; set; } = string.Empty;
    }
}