using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class GroupAdvisor
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public bool GlobalAccess { get; set; }
    }
}
