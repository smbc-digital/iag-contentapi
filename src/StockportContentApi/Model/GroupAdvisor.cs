using System.Collections.Generic;

namespace StockportContentApi.Model
{
    public class GroupAdvisor
    {
        public string Name { get; set; }
        public string EmailAddress { get; set; }
        public IEnumerable<string> Groups { get; set; }
        public bool HasGlobalAccess { get; set; }

        public GroupAdvisor(string name, string emailAddress, IEnumerable<string> groups, bool hasGlobalAccess)
        {
            Name = name;
            EmailAddress = emailAddress;
            Groups = groups;
            HasGlobalAccess = hasGlobalAccess;
        }
    }
}
